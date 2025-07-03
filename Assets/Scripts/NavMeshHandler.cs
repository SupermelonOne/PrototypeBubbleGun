using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshHandler : MonoBehaviour
{
    public NavMeshSurface NavMeshSurface;
    private void Start()
    {
        NavMeshSurface = GetComponent<NavMeshSurface>();
        GameManager.Instance.SetNavHandler(this);
    }
}
