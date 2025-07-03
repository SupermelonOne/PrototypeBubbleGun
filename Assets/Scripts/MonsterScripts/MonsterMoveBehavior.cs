
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class MonsterMoveBehavior : MonoBehaviour
{
    Animator animator;

    public float waitTime;
    public bool isCaught;
    public Transform netPosition;

    private float normalSize = 0.4f;
    private float smallSize = 0.05f;

    [HideInInspector] public bool inStation = false;
    
    [SerializeField] private float walkRange = 50;
    [SerializeField] private float walkWaitTimer = 0;
    [SerializeField] private float walkWaitTime = 10;
    [SerializeField] private float bubbleFloatSpeed = 10f;
    
    private bool goToSecond;
    private Rigidbody rb;
    private NavMeshAgent agent;
    private List<Transform> hidingSpots = new List<Transform>();
    private bool hasLanded = false;

    //for getting random position on navmesh
    private NavMeshSurface navMeshSurface;


    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody>();
        walkWaitTime = Random.Range(7, 12);
        agent = GetComponent<NavMeshAgent>();

        GameObject target = GameObject.Find("HidingSpots");
        if (target != null)
        {
            hidingSpots = target.GetComponentsInChildren<Transform>().Where(t => t != target.transform).ToList();
        }
        else
        {
            Debug.Log("Error: HidingSpots not found");
        }

        bool snappedSuccessfully = TryReconnectAgentToNavMesh();

        // /only set destination if the agent is successfully placed on the NavMesh
        if (snappedSuccessfully && hidingSpots.Count > 0)
        {
            //Debug.Log($"agent is acive: {agent.isActiveAndEnabled}, and on the floor: {agent.isOnNavMesh}");
            agent.SetDestination(GetNearestHidingspot());
        }
        else if (snappedSuccessfully)
        {
            //Debug.Log("Monster snapped to NavMesh but no hiding spots found. Starting wander behavior.", this);
            // If no hiding spots, directly initiate wandering
            walkWaitTimer = walkWaitTime; // Make it pick a random point immediately
        }

    }

    public void EnterStation()
    {
        inStation = true;
        agent.enabled = false;
    }

    public void ExitStation()
    {
        inStation = false;
        agent.enabled = true;
    }

    private void Update()
    {
        UpdateAnimator();

        if (isCaught)
        {
            InNet();
            return;
        }

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }
        
        //pretty sure this isnt how Time.deltaTime works, if this function doesnt work this is prolly the reason
        walkWaitTimer += Time.deltaTime;
        
        if (walkWaitTimer > walkWaitTime)
        {
            walkWaitTimer = 0;
            if (GetRandomPointOnNavmesh(transform.position, walkRange, out var randomPoint))
            {
                //Debug.Log($"agent is active: {agent.isActiveAndEnabled}, and on the floor: {agent.isOnNavMesh}");
                if (agent.isOnNavMesh) 
                    agent.SetDestination(randomPoint);
            }
        }
        
        if (transform.localScale.x < normalSize)
        {
            var growAmount = (0.5f * normalSize)* Time.deltaTime;
            ResizeMonster(growAmount);
        }
    }

    private void InNet()
    {
        if (netPosition is null) return;
        
        transform.position = netPosition.position;
        
        if (inStation)
        {
            if (transform.localScale.x < normalSize)
            {
                var growAmount = (0.5f * normalSize) * Time.deltaTime;
                ResizeMonster(growAmount);
            }
        }
        else
        {
            if (transform.localScale.x > smallSize)
            {
                var shrinkAmount = -(0.5f * normalSize)* Time.deltaTime;
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
//        Debug.Log("amount of hiding spots = " + hidingSpots.Count);
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
    
    
    public bool TryReconnectAgentToNavMesh(float searchRadius = 1)
    {
        if (agent == null)
            return false;
        return agent.isOnNavMesh;
    }

    private void UpdateAnimator()
    {
        if (inStation)
        {
            animator.SetBool("caught", false);
            animator.SetBool("walking", false);
        }
        else
        {
            animator.SetBool("caught", netPosition != null);
            if (agent.isOnNavMesh)
            {
                animator.SetBool("walking", !agent.pathPending &&
           agent.remainingDistance > agent.stoppingDistance &&
           agent.velocity.sqrMagnitude > 0f);
            }
        }

    }
}
