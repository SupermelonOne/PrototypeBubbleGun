using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// All purpose GameManager script, if this gets too full we should split it up
/// </summary>

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Points { get; private set; }
    
    private List<BridgeScript> bridges = new List<BridgeScript>();
    
    NavMeshDataInstance dataInstance;
    NavMeshSurface globalSurface;


    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        globalSurface = GetComponent<NavMeshSurface>();
    }

    public void AddBridge(BridgeScript bridge)
    {
        bridges.Add(bridge);
    }

    public void SetNavMesh()
    {
        Debug.Log("SetNavMesh");
        foreach (BridgeScript bridge in bridges)
        {
            bridge.SetThisNav();
        }

        globalSurface.BuildNavMesh();

        foreach (var bridge in bridges)
        {
            //bridge.RemoveMesh();
        }
    }
}

