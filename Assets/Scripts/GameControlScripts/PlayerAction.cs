using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(AudioSource))]
public abstract class PlayerAction : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public Camera cam;
    
    [SerializeField] private LayerMask layerMask;
    
    [HideInInspector] public Vector3 raycastPosition;

    private bool holding;
    private Coroutine sprayCoroutine;


    public void OnFire(InputAction.CallbackContext button)
    {
        if (!enabled)
            return;

        if (button.started)
        {
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
        var ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            raycastPosition = hit.point;
        }
        else
            raycastPosition = ray.origin + ray.direction * 15f;
        
        
        if (hit.collider != null)
            OnMonsterCast(hit);
    }
}
