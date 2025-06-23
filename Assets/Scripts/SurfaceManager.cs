using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class SurfaceManager : MonoBehaviour
{
    public static SurfaceManager Instance { get; private set; }

    private static NavMeshSurface _surface;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _surface = GetComponent<NavMeshSurface>();
    }

    public void RebuildNavMesh()
    {
        _surface.BuildNavMesh();
    }
}