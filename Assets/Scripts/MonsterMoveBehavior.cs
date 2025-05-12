using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMoveBehavior : MonoBehaviour
{
    NavMeshAgent agent;
    List<Transform> hidingSpots = new List<Transform>();

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            this.AddComponent<NavMeshAgent>();
        }
        GameObject target = GameObject.Find("HidingSpots");
        if (target != null)
        {
            hidingSpots = target.GetComponentsInChildren<Transform>().ToList();
        }
        else
        {
            Debug.Log("Error: HidingSpots not found");
        }
    }

    private void Update()
    {
        agent.SetDestination(GetRandomPointOnNavmesh);
    }

    public void GetTargeted()
    {
        if (agent != null)
        {
            agent.SetDestination(GetNearestHidingspot());
        }
    }

    private Vector3 GetNearestHidingspot()
    {
        // TODO replace this function with a function calling from some event system or something, so not every enemy has to hold the entire array of hiding spots
        return Vector3.zero;

    }
}
