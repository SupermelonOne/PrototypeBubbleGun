using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class Curve : MonoBehaviour
{
    public List<Vector3> points;

    public event Action OnApply;

    public int NumPoints()
    {
        return points.Count;
    }

    public Vector3 GetPoint(int pointIndex)
    {
        if (pointIndex < 0 || pointIndex >= points.Count)
        {
            Debug.Log("Curve.cs: WARNING: pointIndex out of range: " + pointIndex + " curve length: " + points.Count);
            return Vector3.zero;
        }
        return transform.TransformPoint(points[pointIndex]);
    }

    public void Apply()
    {
        MeshCreator creator = GetComponent<MeshCreator>();
        if (creator != null)
        {
            creator.RecalculateMesh();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(Vector3.zero, .5f);
        Handles.Label(new Vector3(10, 10, 10), "banana");
    }
}

