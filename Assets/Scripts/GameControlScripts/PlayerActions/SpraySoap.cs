using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpraySoap : PlayerAction
{
    [SerializeField] private Transform origin;
    [SerializeField] private GameObject streamObject;
    [SerializeField] private GameObject soapObj;
    private float waterLength;
    private Vector3 sprayEndPoint;


    [SerializeField] private List<Transform> sprayEndPointObjects = new List<Transform>();

    private List<ParticleSystem> sprayParticles = new List<ParticleSystem>();
    private void OnEnable()
    {
        StopShooting();
    }
    private void OnDisable()
    {
        StopShooting();
    }

    private void Awake()
    {
        sprayParticles = streamObject.GetComponentsInChildren<ParticleSystem>().ToList();
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
        soapObj.SetActive(true);
        foreach (var particle in sprayParticles)
            particle.Play();
        waterLength = 0;
    }

    protected override void StopShooting()
    {
        foreach (var particle in sprayParticles)
            particle.Stop();
        soapObj.SetActive(false);
    }
}
