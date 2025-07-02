using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootToFinish : MonoBehaviour
{
    StationManager stationManager;
    private void Start()
    {
        if (stationManager == null)
            stationManager = FindObjectOfType<StationManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cleaner") || other.CompareTag("Soap") || other.CompareTag("WaterSpray"))
        {
            stationManager.ReadyCurrentMonster();
        }
    }
}
