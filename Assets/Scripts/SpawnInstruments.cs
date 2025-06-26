using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInstruments : MonoBehaviour
{
    [SerializeField] private Transform origin;
    [SerializeField] private Transform parentObj;
    private void Start()
    {
        if (origin == null)
            origin = transform;
        if (parentObj == null)
            parentObj = transform;
    }

    public void SpawnObjects(List<GameObject> objectsToSpawn)
    {
        foreach(GameObject obj in objectsToSpawn)
        {
            GameObject newObject = Instantiate(obj);
            newObject.transform.position = origin.position;
            newObject.transform.parent = parentObj;
        }
    }
    public GameObject SpawnObject(GameObject objectToSpawn)
    {
        GameObject newObject = Instantiate(objectToSpawn);
        newObject.transform.position = origin.position;
        newObject.transform.parent = parentObj;

        return newObject;
    }

    void Update()
    {
        
    }
}
