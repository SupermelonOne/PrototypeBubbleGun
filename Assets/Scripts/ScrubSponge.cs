using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScrubSponge : MonoBehaviour
{
    [SerializeField] private float range = 15;

    [SerializeField] private Transform sponge;

    [SerializeField] private Transform origin;
    private Vector3 outPoint = Vector3.zero;

    [SerializeField] private LayerMask layersToSpray;

    [SerializeField] Camera cam;

    private float waterLength = 0;

    Ray ray;
    RaycastHit hit;

    private bool holding = false;


    [SerializeField] private float scrubSpeed = 0.1f;
    [SerializeField] private float scrubIntensity = 0.4f;
    private float scrubTimer = 0;
    private Vector3 realDestination;

    // TODO check if this works, and set raycast to each person's camera (maybe it does it automatically, probably doesn't)
    public void OnFire(InputAction.CallbackContext button)
    {
        if (button.started)
        {
            holding = true;
            StartShooting();
        }
        if (button.canceled)
        {
            holding = false;
            StopShooting();
        }
    }

    private void StartShooting()
    { 

    }
    private void StopShooting()
    {
        
    }

    private void Start()
    {
        if (origin == null) origin = transform;
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {

        if (holding && sponge != null)
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            Vector3 hitPoint = ray.direction * 15;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layersToSpray))
            {
                hitPoint = hit.point;
            }

            scrubTimer += Time.deltaTime;
            if (scrubTimer > scrubSpeed)
            {
                scrubTimer = 0;
                realDestination = GetRandomVector(hitPoint, scrubIntensity);
            }
            sponge.position = Vector3.Slerp(sponge.position, realDestination, Time.deltaTime * 10f);
        }
        else
        {
            sponge.position = Vector3.Slerp(sponge.position, origin.position, Time.deltaTime * 10f);
        }
    }

    private Vector3 GetRandomVector (Vector3 input, float distance)
    {

        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        input += randomDirection * distance;
        return input;
    }
}
