using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    private float lastFireTime = -float.MaxValue;

    [SerializeField] private float fireCooldown = 1f;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private GameObject spawnAreaObject; // Assign the Plane or area GameObject here

    private Vector3 areaCenter;
    private Vector3 areaSize;

    private void Start()
    {
        // Try to get bounds from Renderer or Collider
        if (spawnAreaObject.TryGetComponent(out Renderer rend))
        {
            areaSize = rend.bounds.size;
            areaCenter = rend.bounds.center;
        }
        else if (spawnAreaObject.TryGetComponent(out Collider col))
        {
            areaSize = col.bounds.size;
            areaCenter = col.bounds.center;
        }
        else
        {
            Debug.LogWarning("Spawn area object has no Renderer or Collider to get bounds from.");
            areaSize = new Vector3(10f, 0f, 10f);
            areaCenter = spawnAreaObject.transform.position;
        }
    }
    private void Update()
    {
        SpawnMonster();
    }

    private void SpawnMonster()
    {
        if (Time.time >= lastFireTime + fireCooldown){
            lastFireTime = Time.time;
            var posX = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
            var posZ = Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);
            var pos = new Vector3(posX, transform.position.y, posZ);
            
            Instantiate(monsterPrefab, pos, Quaternion.identity);
        }
    }
}
