using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtSpawner : MonoBehaviour
{
    [SerializeField] private Transform monster;
    [SerializeField] List<GameObject> dirtTypes = new List<GameObject>();
    [SerializeField] private float spawnChance = 50;

    private void Start()
    {
        if (Random.Range(0, 100) < spawnChance)
        {
            GameObject newDirt = Instantiate(dirtTypes[Random.Range(0, dirtTypes.Count)], monster);
            newDirt.transform.position = transform.position;

        }
        //Debug.Log("atleast I tried");
    }
}
