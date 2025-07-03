    using UnityEngine;
    using System.IO.Ports;
    using System.Threading;

    using System.Collections.Concurrent;
    using UnityEngine.InputSystem;
    using Unity.Mathematics;

    public class ArduinoInputManager : MonoBehaviour
    {
        public string portName = "COM5";
        public int baudRate = 9600;
        public string lateststring;
        private SerialPort serialPort;
        private Thread readThread;
        private bool isRunning = false;
        public Vector2 Joystick1 = Vector2.zero;
        public Vector2 Joystick2 = Vector2.zero;
        public Vector3 gyroscope = Vector3.zero;
        public bool _button1 = false;
        public bool _button1_pressed = false;
        public bool _button1_hold = false;
        public bool _button1_released = false;
        public bool _button2 = false;
        public bool _button2_pressed = false;
        public bool _button2_hold = false;
        public bool _button2_released = false;
        public bool _button3 = false;
        public bool _button3_pressed = false;
        public bool _button3_hold = false;
        public bool _button3_released = false;
        float counter = 0;

        private ConcurrentQueue<string> lineQueue = new ConcurrentQueue<string>();

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 100;

            try
            {
                serialPort.Open();
                isRunning = true;
                readThread = new Thread(ReadSerialLoop);
                readThread.Start();
                Debug.Log("Serial port opened and reading thread started.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to open serial port: " + e.Message);
            }
        }

        private void ReadSerialLoop()
        {
            while (isRunning && serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    lateststring = serialPort.ReadLine().Trim();

                    ParseData(lateststring);
                }
                catch (System.TimeoutException)
                {
                    // Just continue on timeout
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Serial read error: " + e.Message);
                }

            }
        }

        private void Update()
        {
            CheckPressed(_button1, ref _button1_pressed, ref _button1_hold, ref _button1_released);
            CheckPressed(_button2, ref _button2_pressed, ref _button2_hold, ref _button2_released);
            CheckPressed(_button3, ref _button3_pressed, ref _button3_hold, ref _button3_released);
        }

        private void CheckPressed(bool p_button, ref bool _pressed, ref bool _hold, ref bool _released)
        {
            if (p_button && !_hold)
            {
                _pressed = true;
                _hold = true;
            }
            else if (p_button)
            {
                _pressed = false;
                _hold = true;
            }
            else if (_hold)
            {
                _released = true;
                _hold = false;
            }
            else
            {
                _released = false;
            }
        }

    private void ParseData(string line)
    {
        string[] tokens = line.Split(',');

        if (tokens.Length < 10)
        {
            Debug.LogWarning($"Malformed input (expected 10 tokens, got {tokens.Length}): {line}");
            return;
        }

        // Joysticks (still using ints)
        int joy1x = SafeParseInt(tokens[0]);
        int joy1y = SafeParseInt(tokens[1]);
        int joy2x = SafeParseInt(tokens[2]);
        int joy2y = SafeParseInt(tokens[3]);

        Joystick1 = ClampInputPair(joy1y, joy1x, 750f, 10f, false, false);
        Joystick2 = ClampInputPair(joy2y, joy2x, 750f, 10f, true, true);
        counter++;

        // Gyroscope (now robust with float parsing)
        float gyroX = SafeParseFloat(tokens[5]);
        float gyroY = SafeParseFloat(tokens[6]);
        float gyroZ = SafeParseFloat(tokens[4]);

        gyroscope = new Vector3(gyroX, gyroY, gyroZ);   

        // Buttons
        int button1 = SafeParseInt(tokens[7]);
        int button2 = SafeParseInt(tokens[8]);
        int button3 = SafeParseInt(tokens[9]);

        if (button1 == 1) OnButton1();
        _button1 = button1 == 1;
        if (button2 == 1) OnButton2();
        _button2 = button2 == 1;
        if (button3 == 1) OnButton3();
        _button3 = button3 == 1;
    }


    // General normalization for 2D input (like joysticks, accel, gyro)

    private Vector2 ClampInputPair(
            float x, float y,
            float center = 0f,
            float deadzone = 0f,
            bool invertX = false, bool invertY = false)
        {
            float ClampToUnit(float value, bool invert)
            {
                float adjusted = value - center;
                if (Mathf.Abs(adjusted) <= deadzone) return 0f;

                // Optionally clamp to [-1, 1] range
                float clamped = Mathf.Clamp(adjusted / 512f, -1f, 1f); // scale from ~-512 to 512 if 10-bit ADC centered at 512–750
                return invert ? -clamped : clamped;
            }

            return new Vector2(ClampToUnit(x, invertX), ClampToUnit(y, invertY));
        }

        private Vector3 NormalizeInputTriple(
            float x, float y, float z,
            float center = 0f, float min = -1f, float max = 1f,
            float deadzone = 0f,
            bool invertX = false, bool invertY = false, bool invertZ = false)
        {
            float Normalize(float value, bool invert)
            {
                float normalized = ((value - center) / (max - center)) * 100f;
                if (Mathf.Abs(normalized) <= deadzone) return 0f;
                normalized = Mathf.Clamp(normalized, -100f, 100f);
                return invert ? -normalized : normalized;
            }

            return new Vector3(
                Normalize(x, invertX),
                Normalize(y, invertY),
                Normalize(z, invertZ)
            );
        }

        private Vector3 ClampInputTriple(
            float x, float y, float z,
            float center = 0f,
            float deadzone = 0f,
            bool invertX = false, bool invertY = false, bool invertZ = false)
        {
            float ClampToRange(float value, bool invert)
            {
                float adjusted = value - center;

                if (Mathf.Abs(adjusted) <= deadzone) return 0f;

                // Clamp to realistic gyro bounds (e.g. -180 to 180 degrees/sec)
                float clamped = Mathf.Clamp(adjusted, -180f, 180f);
                return invert ? -clamped : clamped;
            }

            return new Vector3(
                ClampToRange(x, invertX),
                ClampToRange(y, invertY),
                ClampToRange(z, invertZ)
            );
        }

    private float SafeParseFloat(string str, float fallback = 0f)
    {
        if (float.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float result))
            return result;

        Debug.LogWarning($"Failed to parse float from: '{str}'");
        return fallback;
    }

    private int SafeParseInt(string str, int fallback = 0)
    {
        if (int.TryParse(str, out int result))
            return result;

        Debug.LogWarning($"Failed to parse int from: '{str}'");
        return fallback;
    }







    private void OnButton1()
        {
            //Debug.Log("Button 1 Pressed!");
            // Add your logic here
        }

        private void OnButton2()
        {
            //Debug.Log("Button 2 Pressed!");
            // Add your logic here
        }

        private void OnButton3()
        {
            //Debug.Log("Button 3 Pressed!");
            // Add your logic here
        }

        void OnApplicationQuit()
        {
            Stop();
        }

        public void Stop()
        {
            isRunning = false;
            if (readThread != null && readThread.IsAlive)
            {
                readThread.Join();
            }

            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }

            Debug.Log("Serial port closed and thread stopped.");
        }
    }
