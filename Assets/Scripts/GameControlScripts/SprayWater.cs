using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SprayWater : PlayerAction
{
    [SerializeField] private Transform origin;
    [SerializeField] private GameObject streamObject;
    private GameObject waterStream = null;
    private float waterLength = 0;
    private Vector3 sprayEndPoint;
    private Coroutine sprayCoroutine;

    
    // TODO check if this works, and set raycast to each person's camera (maybe it does it automatically, probably doesn't)



    public override void OnMonsterCast(RaycastHit hit)
    {
        sprayEndPoint = hit.point;
    }



    public override void ButtonDown()
    {
        if (waterStream == null)
            return;
        
        waterStream.transform.position = origin.position;

        sprayEndPoint = cam.transform.forward * 50;
        
        waterStream.transform.forward = (sprayEndPoint - waterStream.transform.position).normalized;
        float distance = (sprayEndPoint - waterStream.transform.position).magnitude/2;
        if (waterLength < distance)
        {
            waterLength += 100f * Time.deltaTime;
        }
        if (waterLength > distance)
        {
            waterLength = distance;
        }
        waterStream.transform.localScale = new Vector3(1, 1, waterLength);
        
    }

    public override void StartShooting()
    {
        waterStream = Instantiate(streamObject);
        waterLength = 0;
    }
    public override void StopShooting()
    {
        if (waterStream != null)
        {
            List<ParticleSystem> particleSystems = waterStream.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>();
            foreach (ParticleSystem particle in particleSystems)
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
