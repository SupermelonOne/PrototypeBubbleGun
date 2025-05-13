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
    public bool canMove = true;
    [SerializeField] private float walkRange = 10;

    [SerializeField] private float walkWaitTimer = 0;
    [SerializeField] private float walkWaitTime = 10;

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
            hidingSpots = target.GetComponentsInChildren<Transform>().Where(t => t != target.transform).ToList();
        }
        else
        {
            Debug.Log("Error: HidingSpots not found");
        }
    }

    private void Update()
    {
        if (!canMove)
        {
            agent.isStopped = true;
            return;
        }
        agent.isStopped = false;
        walkWaitTimer += Time.deltaTime;
        if (walkWaitTimer > walkWaitTime)
        {
            walkWaitTimer = 0;
            Vector3 randomPoint;
            if (GetRandomPointOnNavmesh(transform.position, walkRange, out randomPoint))
            {
                agent.SetDestination(randomPoint);
            }
        }
    }
    bool GetRandomPointOnNavmesh(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++) // Try multiple times in case of failure
        {
            Vector3 randomPos = center + Random.insideUnitSphere * range;
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }

    public void Hide()
    {
        if (agent != null)
        {
            walkWaitTimer = 0;
            agent.SetDestination(GetNearestHidingspot());
        }
    }

    private Vector3 GetNearestHidingspot()
    {
        Vector3 placeToGo = Vector3.zero;
        float distanceToPlace = Mathf.Infinity;

        foreach (Transform hideSpot in hidingSpots)
        {
            float distance = Vector3.Distance(hideSpot.position, transform.position);

            if (distance < distanceToPlace)
            {
                placeToGo = hideSpot.position;
                distanceToPlace = distance;
            }
        }
        return placeToGo;
    }
}
