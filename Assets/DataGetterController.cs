using UnityEngine;
using System.IO.Ports;
using System.Threading;

using System.Collections.Concurrent;
using UnityEngine.InputSystem;

public class ArduinoInputManager : MonoBehaviour
{
    public string portName = "COM5";
    public int baudRate = 9600;
    public string lateststring;
    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;
    private Vector2 Joystick1 = Vector2.zero;
    private Vector2 Joystick2 = Vector2.zero;
    private Vector3 gyroscope = Vector3.zero;
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


    private void ParseData(string line)
    {
        string[] tokens = line.Split(',');

        if (tokens.Length < 10) return;

        // Joystick values
        int joy1x = int.Parse(tokens[0]);
        int joy1y = int.Parse(tokens[1]);
        int joy2x = int.Parse(tokens[2]);
        int joy2y = int.Parse(tokens[3]);
        Joystick1 = NormalizeInputPair(joy1y, joy1x, 750f, 0f, 1023f, 10f, false, false);
        Joystick2 = NormalizeInputPair(joy2y, joy2x, 750f, 0f, 1023f, 10f, true, true);
        counter++;

        //Debug.Log("joy1"+ Joystick1+ counter);
        //Debug.Log("joy2"+ Joystick2 + counter );
        // Gyroscope
        float gyroX = float.Parse(tokens[4]);
        float gyroY = float.Parse(tokens[5]);
        float gyroZ = float.Parse(tokens[6]);
        gyroscope = NormalizeInputTriple(gyroX, gyroY, gyroZ, 0f, -200f, 200f, 10f, false, false, false);
        Debug.Log("Gyroscope: " + gyroscope.ToString("F2") + " | Counter: " + counter);
        // Buttons
        int button1 = int.Parse(tokens[7]);
        int button2 = int.Parse(tokens[8]);
        int button3 = int.Parse(tokens[9]);

        // Debug.Log($"Joy1: ({joy1x}, {joy1y}) | Joy2: ({joy2x}, {joy2y}) | " +
        //          $"Gyro: ({gyroX}, {gyroY}, {gyroZ}) | Accel: ({accelX}, {accelY}, {accelZ}) | " +
        //          $"Buttons: {button1}, {button2}, {button3}");

        if (button1 == 1) OnButton1();
        if (button2 == 1) OnButton2();
        if (button3 == 1) OnButton3();
    }

    // General normalization for 2D input (like joysticks, accel, gyro)

    private Vector2 NormalizeInputPair(
        float x, float y,
        float center = 0f, float min = -1f, float max = 1f,
        float deadzone = 0f,
        bool invertX = false, bool invertY = false)
    {
        float Normalize(float value, bool invert)
        {
            float normalized = ((value - center) / (max - center)) * 100f;
            if (Mathf.Abs(normalized) <= deadzone) return 0f;
            normalized = Mathf.Clamp(normalized, -100f, 100f);
            return invert ? -normalized : normalized;
        }

        return new Vector2(Normalize(x, invertX), Normalize(y, invertY));
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






    private void OnButton1()
    {
        Debug.Log("Button 1 Pressed!");
        // Add your logic here
    }

    private void OnButton2()
    {
        Debug.Log("Button 2 Pressed!");
        // Add your logic here
    }

    private void OnButton3()
    {
        Debug.Log("Button 3 Pressed!");
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
