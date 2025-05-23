
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

public class MonsterMoveBehavior : MonoBehaviour
{
    NavMeshAgent agent;
    List<Transform> hidingSpots = new List<Transform>();
    public float waitTime = 0;
    [SerializeField] private float walkRange = 10;

    [SerializeField] private float walkWaitTimer = 0;
    [SerializeField] private float walkWaitTime = 10;

    [SerializeField] private float bubbleFloatSpeed = 10f;
    Rigidbody rb;

    public bool isCaught = false;
    public Transform netPosition;

    [SerializeField] private bool goToSecond = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        walkWaitTime = Random.Range(7, 12);
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            this.AddComponent<NavMeshAgent>();
        }
        GameObject target = GameObject.Find("HidingSpots");
        if (target != null)
        {
            hidingSpots = target.GetComponentsInChildren<Transform>().Where(t => t != target.transform).ToList();
            foreach(Transform point in hidingSpots)
            {
                if (goToSecond)
                Debug.Log("position is at: " + point.position);
            }
        }
        else
        {
            Debug.Log("Error: HidingSpots not found");
        }
    }

    private void Update()
    {
        if (!isCaught)
        {
            if (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                transform.position += new Vector3(0, bubbleFloatSpeed * Time.deltaTime, 0);
                return;
            }
            if (rb != null && !rb.useGravity)
            {
                rb.useGravity = true;
                agent.enabled = true;    ;
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
            if (transform.localScale.x < 0.9f)
            {
                Vector3 scale = new Vector3(
                    transform.localScale.x + 0.5f * Time.deltaTime,
                    transform.localScale.y + 0.5f * Time.deltaTime,
                    transform.localScale.z + 0.5f * Time.deltaTime
                    );
                transform.localScale = scale;
            }
        }
        else if (netPosition != null)
        {
            transform.position = netPosition.position;
            if (transform.localScale.x > 0.5f)
            {
                float shrinkAmount = 0.5f * Time.deltaTime;
                Debug.Log(shrinkAmount);
                Vector3 scale = new Vector3(
                    transform.localScale.x - shrinkAmount, 
                    transform.localScale.y - shrinkAmount, 
                    transform.localScale.z - shrinkAmount
                    );
                transform.localScale = scale;
            }
        }

        if (agent.isStopped)
        {
            goToSecond = false;
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
        Vector3 secondPlace = Vector3.zero;
        float distanceToSecond = Mathf.Infinity;

        int howMany = 0;

        foreach (Transform hideSpot in hidingSpots)
        {
            float distance = Vector3.Distance(hideSpot.position, transform.position);
            howMany++;
            if (distance < distanceToPlace)
            {
                secondPlace = placeToGo;
                distanceToSecond = distanceToPlace;
                placeToGo = hideSpot.position;
                distanceToPlace = distance;
            }
        }
        Debug.Log("how many = " + howMany);
        if (goToSecond || distanceToPlace < 3)
        {
            goToSecond = true;
            Debug.Log(distanceToPlace);
            Debug.Log(placeToGo);
            Debug.Log(secondPlace);
            placeToGo = secondPlace;
        }
        return placeToGo;
    }

    public void StopMoving(float stopTime)
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
        waitTime = stopTime;
        walkWaitTimer = walkWaitTime;
    }

    public void Capture(Transform transformToFollow)
    {
        netPosition = transformToFollow;
        isCaught = true;
        //Destroy(GetComponent<Collider>());
/*        if (agent != null)
        {
            agent.enabled = false;
        }
        Destroy(GetComponent<NavMeshAgent>());*/
    }
    public void Release()
    {
        isCaught = false;
        netPosition = null;
    }
}
