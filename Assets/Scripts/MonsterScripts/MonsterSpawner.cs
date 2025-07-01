using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    private float lastFireTime = -float.MaxValue;

    [SerializeField] private float fireCooldown = 1f;
    [SerializeField] private GameObject monsterPrefab;

    [SerializeField] private Sprite[] sprites;

    [SerializeField] private int monsterCap = 10;
    private List<GameObject> spwanedMonsters = new List<GameObject>();
    private Vector3 areaCenter;
    private Vector3 areaSize;
    

    private void Start()
    {
        GameObject spawnAreaObject;
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
        {
            spawnAreaObject = hit.collider.gameObject;
        }
        else
        {
            return;
        }
        
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
        if (spwanedMonsters.Count <= monsterCap)
            SpawnMonster();
    }


    private void SpawnMonster()
    {
        if (!(Time.time >= lastFireTime + fireCooldown)) return;
        lastFireTime = Time.time;

        float posX = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
        float posZ = Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);
        Vector3 randomPos = new Vector3(posX, transform.position.y + 10f, posZ);

        if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, 50f, LayerMask.GetMask("Surface")))
        {
            Vector3 groundPos = hit.point;

            // 🔍 Check for NavMesh position near groundPos
            if (NavMesh.SamplePosition(groundPos, out NavMeshHit navHit, 1, NavMesh.AllAreas))
            {
                Vector3 spawnPos = navHit.position;

                GameObject m = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
                m.transform.parent = transform;

                SpriteRenderer monsterRend = m.GetComponentInChildren<SpriteRenderer>();
                monsterRend.sprite = sprites[Random.Range(0, sprites.Length)];

                // Offset so it's visually above ground
                float spriteHeight = monsterRend.bounds.size.y;
                m.transform.position += new Vector3(0, spriteHeight / 2 + 0.01f, 0);

                spwanedMonsters.Add(m);
            }
            else
            {
                Debug.LogWarning("No NavMesh found near spawn position. Skipping spawn.");
            }
        }
    }


}
