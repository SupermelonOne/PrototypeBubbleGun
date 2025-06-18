using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds a flat mesh between pairs of points.
/// Assumes input is an even number of points: [L0, R0, L1, R1, ..., Ln, Rn]
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RectMeshBuilder : MonoBehaviour
{
    public void SetMesh(List<Vector3> worldVerts)
    {
        if (worldVerts.Count < 4 || worldVerts.Count % 2 != 0)
        {
            Debug.LogWarning("Need at least 2 quads (4 points), and even count.");
            return;
        }

        // Convert to local space
        Vector3[] vertices = new Vector3[worldVerts.Count];
        for (int i = 0; i < worldVerts.Count; i++)
        {
            vertices[i] = transform.InverseTransformPoint(worldVerts[i]);
        }


        // Build triangles
        List<int> triangles = new List<int>();
        for (int i = 0; i < vertices.Length - 2; i += 2)
        {
            int i0 = i;
            int i1 = i + 1;
            int i2 = i + 2;
            int i3 = i + 3;

            // Triangle 1: i0, i2, i1
            triangles.Add(i0);
            triangles.Add(i2);
            triangles.Add(i1);

            // Triangle 2: i2, i3, i1
            triangles.Add(i2);
            triangles.Add(i3);
            triangles.Add(i1);
        }

        // Simple UV mapping: spread points along X and Z
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            var v = vertices[i];
            uvs[i] = new Vector2(v.x, v.z); // Adjust based on how you want to project
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = false; // If needed
    }

    public void RemoveMesh()
    {
        GetComponent<MeshFilter>().mesh = null;
        
    }
}