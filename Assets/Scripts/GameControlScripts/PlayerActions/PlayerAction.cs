using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(AudioSource))]
public abstract class PlayerAction : MonoBehaviour
{
    protected ArduinoInputManager inputManager;
    [SerializeField] private string comPort = "COM5";

    private bool _pressed = false;
    private bool _hold = false;
    private bool _released = false;

    private void Awake()
    {
        foreach(ArduinoInputManager im in FindObjectsOfType<ArduinoInputManager>())
        {
            if (im.portName == comPort)
            {
                inputManager = im;
            }
        }
        if (inputManager == null)
            Debug.Log("failed to find controller");
    }

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public Camera cam;
    
    [SerializeField] private LayerMask layerMask;
    
    [HideInInspector] public Vector3 raycastPosition;

    protected bool holding;
    private Coroutine sprayCoroutine;


    public void OnFire(InputAction.CallbackContext button)
    {
        if (!enabled)
            return;

        Debug.Log($"button: {button.phase}");

        if (button.started)
        {
            Debug.Log("Fire");
            holding = true;
            StartShooting();
            sprayCoroutine = StartCoroutine(OnButtonDown());
        }
        if (button.canceled)
        {
            holding = false;
            StopShooting();
            if (sprayCoroutine != null)
            {
                StopCoroutine(sprayCoroutine);
                sprayCoroutine = null;
            }
        }
    }

    protected abstract void OnMonsterCast(RaycastHit hit);
    protected virtual void StartShooting() { }
    protected virtual void StopShooting() { }
    protected virtual void PassiveUpdate() { }

    private IEnumerator OnButtonDown()
    {
        while (holding)
        {
            ButtonDown();
            yield return null; // wait for next frame
        }
    }
    
    protected virtual void ButtonDown(){}
    private void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (cam == null)
        {
            cam = Camera.main;
        }
    }
    
    private void Update()
    {
        //IMPLEMENT NEW CONTROLLER HERE, WITH PROPER PRESSED, HOLD AND RELEASED FUNCTION CALLING
        if (!enabled)
            return;
        if (inputManager != null)
        {
            if (inputManager._button2 && !_hold)
            {
                _pressed = true;
                _hold = true;
                // button.started functionality
                Debug.Log("pressed");
                holding = true;
                StartShooting();
                sprayCoroutine = StartCoroutine(OnButtonDown());
            }
            else if (inputManager._button2)
            {
                _pressed = false;
                _hold = true;
            }
            else if (_hold)
            {
                _released = true;
                _hold = false;
                Debug.Log("released");

                //button.cancelled fuctionality
                holding = false;
                StopShooting();
                if (sprayCoroutine != null)
                {
                    StopCoroutine(sprayCoroutine);
                    sprayCoroutine = null;
                }
            }
            else
            {
                _released = false;
            }
        }

        var ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            raycastPosition = hit.point;
        }
        else
            raycastPosition = ray.origin + ray.direction * 5f;
        
        
        if (hit.collider != null)
            OnMonsterCast(hit);

        //PassiveUpdate();
    }

    private void LateUpdate()
    {
        PassiveUpdate();
    }
}
