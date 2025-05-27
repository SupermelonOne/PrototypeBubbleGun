using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPosition;
    private void Start()
    {
        GameObject respawnObj = GameObject.Find("SpawnPlace");
        respawnPosition = respawnObj.transform;
    }

    private void Update()
    {
        Debug.Log(transform.position.y);

        if (transform.position.y < -90)
        {
            transform.position = respawnPosition.position;
        }
    }
}
