using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SprayWater : PlayerAction
{
    [SerializeField] private Transform origin;
    [SerializeField] private GameObject streamObject;
    private GameObject waterStream;
    private float waterLength;
    private Vector3 sprayEndPoint;
    private Coroutine sprayCoroutine;

    protected override void OnMonsterCast(RaycastHit hit)
    {
        sprayEndPoint = hit.point;
    }
    
    protected override void ButtonDown()
    {
        if (waterStream is null)
            return;
        
        waterStream.transform.position = origin.position;

        sprayEndPoint = cam.transform.forward * 50;
        
        waterStream.transform.forward = (sprayEndPoint - waterStream.transform.position).normalized;
        var distance = (sprayEndPoint - waterStream.transform.position).magnitude/2;
        
        if (waterLength < distance)
            waterLength += 100f * Time.deltaTime;
        else
            waterLength = distance;
        
        waterStream.transform.localScale = new Vector3(1, 1, waterLength);
        
    }

    protected override void StartShooting()
    {
        waterStream = Instantiate(streamObject, transform);
        waterLength = 0;
    }
    
    protected override void StopShooting()
    {
        if (waterStream == null)
            return;
        
        var particleSystems = waterStream.GetComponentsInChildren<ParticleSystem>().ToList();
        foreach (var particle in particleSystems)
            particle.Stop();
        
        var waterStreamObject = waterStream.GetComponentInChildren<MeshBreathe>();
        waterStreamObject.gameObject.SetActive(false);
        Destroy(waterStream, 2);
    }
}
