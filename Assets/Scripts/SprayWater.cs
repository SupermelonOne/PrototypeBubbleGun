using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SprayWater : MonoBehaviour
{
    [SerializeField] private Transform origin;
    private Vector3 outPoint = Vector3.zero;

    [SerializeField] private GameObject streamObject;
    private GameObject waterStream = null;

    [SerializeField] private LayerMask layersToSpray;

    [SerializeField] Camera cam;

    private float waterLength = 0;

    Ray ray;
    RaycastHit hit;

    private void Start()
    {
        if (origin == null) origin = transform;
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            waterStream = Instantiate(streamObject);
            waterLength = 0;
        }
        if (Input.GetMouseButton(0) && waterStream != null)
        {
            waterStream.transform.position = origin.position;


            Ray ray = (cam.ScreenPointToRay(Input.mousePosition));
            RaycastHit hit;

            Vector3 sprayEndPoint = ray.direction * 50;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layersToSpray))
            {
                sprayEndPoint = hit.point;
            }

            waterStream.transform.forward = (sprayEndPoint - waterStream.transform.position).normalized;
            float distance = (sprayEndPoint - waterStream.transform.position).magnitude;
            if (waterLength < distance)
            {
                waterLength += distance * Time.deltaTime;
            }
            if (waterLength > distance)
            {
                waterLength = distance;
            }
            waterStream.transform.localScale = new Vector3(1, 1, waterLength);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (waterStream != null)
            {
                List<ParticleSystem> particleSystems = waterStream.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>();
                foreach(ParticleSystem particle in particleSystems)
                {
                    particle.Stop();
                }
                Destroy(waterStream, 2);
                GameObject waterStreamObject = GameObject.Find("WaterStreamObject");
                if (waterStreamObject != null)
                    Destroy(waterStreamObject);
                ray = new Ray(transform.position, Vector3.down);
                if (Physics.Raycast(ray, out hit, 2f))
                {

                }
            }
        }
    }
}
