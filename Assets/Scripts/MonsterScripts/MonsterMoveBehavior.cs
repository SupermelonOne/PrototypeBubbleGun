
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterMoveBehavior : MonoBehaviour
{
    public float waitTime;
    public bool isCaught;
    public Transform netPosition;
    
    [HideInInspector] public bool inStation = false;
    
    [SerializeField] private float walkRange = 50;
    [SerializeField] private float walkWaitTimer = 0;
    [SerializeField] private float walkWaitTime = 10;
    [SerializeField] private float bubbleFloatSpeed = 10f;
    
    private bool goToSecond;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private List<Transform> hidingSpots = new List<Transform>();


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        walkWaitTime = Random.Range(7, 12);
        agent = GetComponent<NavMeshAgent>();

        /*
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
        }*/
    }

    public void EnterStation()
    {
        inStation = true;
    }

    public void ExitStation()
    {
        inStation = false;
    }

    private void Update()
    {
        goToSecond = !agent.isStopped;

        if (isCaught)
        {
            InNet();
            return;
        }

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            transform.position += new Vector3(0, bubbleFloatSpeed * Time.deltaTime, 0);
            return;
        }
        
        if (!rb.useGravity)
        {
            rb.useGravity = true;
            agent.enabled = true;   
        }
        
        agent.isStopped = false;
        //pretty sure this isnt how Time.deltaTime works, if this function doesnt work this is prolly the reason
        walkWaitTimer += Time.deltaTime;
        
        if (walkWaitTimer > walkWaitTime)
        {
            walkWaitTimer = 0;
            if (GetRandomPointOnNavmesh(transform.position, walkRange, out var randomPoint))
            {
                agent.SetDestination(randomPoint);
            }
        }
        
        if (transform.localScale.x < 0.9f)
        {
            var growAmount = 0.5f * Time.deltaTime;
            ResizeMonster(growAmount);
        }
    }

    private void InNet()
    {
        if (netPosition is null) return;
        
        transform.position = netPosition.position;
        
        if (inStation)
        {
            if (transform.localScale.x < 0.9f)
            {
                var growAmount = 0.5f * Time.deltaTime;
                ResizeMonster(growAmount);
            }
        }
        else
        {
            if (transform.localScale.x > 0.5f)
            {
                var shrinkAmount = -0.5f * Time.deltaTime;
                ResizeMonster(shrinkAmount);
            }
        }

    }

    private void ResizeMonster(float resizeFactor)
    {
        Vector3 scale = new Vector3(
            transform.localScale.x + resizeFactor,
            transform.localScale.y + resizeFactor,
            transform.localScale.z + resizeFactor
        );
        transform.localScale = scale;
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
        walkWaitTimer = 0;
        agent.SetDestination(GetNearestHidingspot());
    }
    
    private Vector3 GetNearestHidingspot()
    {
        var placeToGo = Vector3.zero;
        var distanceToPlace = Mathf.Infinity;
        var secondPlace = Vector3.zero;

        foreach (var hideSpot in hidingSpots)
        {
            var distance = Vector3.Distance(hideSpot.position, transform.position);
            
            if (!(distance < distanceToPlace)) continue;
            
            secondPlace = placeToGo;
            placeToGo = hideSpot.position;
            distanceToPlace = distance;
        }
        Debug.Log("amount of hiding spots = " + hidingSpots.Count);
        if (goToSecond || distanceToPlace < 3)
        {
            goToSecond = true; ;
            placeToGo = secondPlace;
        }
        return placeToGo;
    }
    
    public void StopMoving(float stopTime)
    {
        agent.isStopped = true;
        waitTime = stopTime;
        walkWaitTimer = walkWaitTime;
    }
    
    public void Capture(Transform transformToFollow)
    {
        netPosition = transformToFollow;
        isCaught = true;
    }
    
    public void Release()
    {
        isCaught = false;
        netPosition = null;
    }
}
