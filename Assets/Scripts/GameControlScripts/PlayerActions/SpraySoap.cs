using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpraySoap : PlayerAction
{
    [SerializeField] private Transform origin;
    [SerializeField] private GameObject streamObject;
    [SerializeField] private List<GameObject> soapObj = new List<GameObject>();
    private float waterLength;
    private Vector3 sprayEndPoint;


    [SerializeField] private List<Transform> sprayEndPointObjects = new List<Transform>();

    private List<ParticleSystem> sprayParticles = new List<ParticleSystem>();
    private void OnEnable()
    {
        sprayParticles = streamObject.GetComponentsInChildren<ParticleSystem>().ToList();
        StopShooting();
    }
    private void OnDisable()
    {
        StopShooting();
    }

    protected override void OnMonsterCast(RaycastHit hit)
    {
        sprayEndPoint = hit.point;
    }

    protected override void ButtonDown()
    {
        foreach (Transform t in sprayEndPointObjects)
        {
            t.position = sprayEndPoint;
        }
    }

    protected override void StartShooting()
    {
        foreach(GameObject obj in soapObj)
        {
            obj.SetActive(true);
        }
        foreach (var particle in sprayParticles)
            particle.Play();
        waterLength = 0;
    }

    protected override void StopShooting()
    {
        foreach (var particle in sprayParticles)
            particle.Stop();
        foreach (GameObject obj in soapObj)
        {
            obj.SetActive(false);
        }
    }
}
