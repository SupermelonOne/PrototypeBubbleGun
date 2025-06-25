using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform target;
    public void SetTarget(Transform _target)
    {
        target = _target;
    }
    public void ResetTarget()
    {
        target = null;
    }
    private void Update()
    {
        if (target == null)
            return;
        transform.position = target.position;
    }
}
