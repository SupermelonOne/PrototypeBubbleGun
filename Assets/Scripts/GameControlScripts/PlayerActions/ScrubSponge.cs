using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScrubSponge : PlayerAction
{
    [SerializeField] private List<Transform> sponge = new List<Transform>();
    [SerializeField] private Transform origin;
    [SerializeField] private float scrubSpeed = 0.1f;
    [SerializeField] private float scrubIntensity = 0.4f;
    private float scrubTimer = 0;
    private Vector3 realDestination;
    private Vector3 hitPoint;

    
    protected override void ButtonDown()
    {
        if (sponge == null) return;

        hitPoint = raycastPosition; //cam.transform.forward * 15;
        scrubTimer += Time.deltaTime;
        if (scrubTimer > scrubSpeed)
        {
            scrubTimer = 0;
            realDestination = GetRandomVector(hitPoint, scrubIntensity);
        }
        foreach(Transform t in sponge)
        {
            t.position = Vector3.Lerp(t.position, realDestination, Time.deltaTime * 10f);
        }
    }

    protected override void StopShooting()
    {
        //sponge.position = Vector3.Slerp(sponge.position, origin.position, Time.deltaTime * 10f);
    }
    protected override void PassiveUpdate()
    {
        if (!holding)
        {
            foreach (Transform t in sponge)
            {
                t.position = Vector3.Lerp(t.position, origin.position, Time.deltaTime * 10f);
            }
        }
    }

    protected override void OnMonsterCast(RaycastHit hit)
    {
        hitPoint = hit.point;
    }

    private Vector3 GetRandomVector (Vector3 input, float distance)
    {
        Vector3 randomDirection = Random.insideUnitCircle.normalized;
        input += randomDirection * distance;
        return input;
    }
}
