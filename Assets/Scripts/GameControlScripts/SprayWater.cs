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
    private float waterLength = 0;
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
        waterStream = Instantiate(streamObject);
        waterLength = 0;
    }
    
    protected override void StopShooting()
    {
        if (waterStream != null)
        {
            var particleSystems = waterStream.GetComponentsInChildren<ParticleSystem>().ToList();
            foreach (var particle in particleSystems)
            {
                particle.Stop();
            }
            
            //TODO: fix this abomination 
            Destroy(waterStream, 2);
            GameObject waterStreamObject = GameObject.Find("WaterStreamObject");
            if (waterStreamObject != null)
                Destroy(waterStreamObject);
            waterStreamObject = GameObject.Find("WaterStreamObject");
            if (waterStreamObject != null)
                Destroy(waterStreamObject);
        }
    }
}
