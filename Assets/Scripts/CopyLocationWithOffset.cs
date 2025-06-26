using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// leave offset empty to automatically calculate it on startup
public class CopyLocationWithOffset : MonoBehaviour
{
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Transform original;
    [SerializeField] private Transform target;

    private void Start()
    {
        if (offset == Vector3.zero)
        {
            offset = target.position - original.position;
        }
    }
    private void Update()
    {
        if (original == null || target == null)
            return;
        target.transform.position = original.position + offset;
    }
}
