// Version 2023
//  (Updates: no getters in loops)
using UnityEngine;

public class AutoUv : MonoBehaviour
{
    public Vector2 textureScaleFactor = new Vector2(1, 1);
    public bool UseWorldCoordinates;
    public bool AutoUpdate;
    public bool RecalculateTangents = true;

    void Update()
    {
        if ((transform.hasChanged && UseWorldCoordinates && AutoUpdate) || Input.GetKeyDown(KeyCode.F2))
        {
            UpdateUvs();
            transform.hasChanged = false;
        }
    }

    public void UpdateUVs(Mesh mesh)
    {
        Debug.Log("Updating UVs");

        Vector2[] uv = mesh.uv;
        int[] tris = mesh.triangles;
        Vector3[] verts = mesh.vertices;

        // New: Accumulators for tangent and bitangent
        Vector3[] tangentSum = new Vector3[mesh.vertexCount];
        Vector3[] bitangentSum = new Vector3[mesh.vertexCount];
        int[] contributionCount = new int[mesh.vertexCount];

        for (int i = 0; i < tris.Length; i += 3)
        {
            int i1 = tris[i];
            int i2 = tris[i + 1];
            int i3 = tris[i + 2];
            Vector3 v1 = verts[i1];
            Vector3 v2 = verts[i2];
            Vector3 v3 = verts[i3];
            if (UseWorldCoordinates)
            {
                v1 = transform.TransformPoint(v1);
                v2 = transform.TransformPoint(v2);
                v3 = transform.TransformPoint(v3);
            }
            Vector3 tangent;
            Vector3 biTangent;
            // TODO: Take vertices that are part of multiple triangles + slight mesh warping into account.
            //  Possible solution:
            //   Store the computed tangent & bitangent for each vertex.
            //   If those have already been computed for at least one of the triangle vertices, assign those to all triangle vertices, instead of recomputing.
            ComputeTangents(v1, v2, v3, out tangent, out biTangent);

            // Accumulate tangents and bitangents per vertex
            tangentSum[i1] += tangent;
            tangentSum[i2] += tangent;
            tangentSum[i3] += tangent;

            bitangentSum[i1] += biTangent;
            bitangentSum[i2] += biTangent;
            bitangentSum[i3] += biTangent;

            contributionCount[i1]++;
            contributionCount[i2]++;
            contributionCount[i3]++;
            //ComputeTriangleUVs(v1, v2, v3, ref uv[i1], ref uv[i2], ref uv[i3], tangent, biTangent);
        }

        // Generate UVs using averaged tangent/bitangent vectors
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = UseWorldCoordinates ? transform.TransformPoint(verts[i]) : verts[i];

            if (contributionCount[i] > 0)
            {
                Vector3 avgTangent = tangentSum[i] / contributionCount[i];
                Vector3 avgBitangent = bitangentSum[i] / contributionCount[i];

                uv[i] = new Vector2(Vector3.Dot(v, avgTangent), Vector3.Dot(v, avgBitangent)) / textureScaleFactor;
            }
            else
            {
                uv[i] = Vector2.zero;
            }
        }
        mesh.uv = uv;
        if (RecalculateTangents)
        {
            mesh.RecalculateTangents();
        }
    }

    public void UpdateUvs()
    {
        // Clone the shared mesh manually, to prevent the "leaking meshes" error:
        Mesh origMesh = GetComponent<MeshFilter>().sharedMesh;
        Mesh mesh = (Mesh)Instantiate(origMesh);

        UpdateUVs(mesh);

        GetComponent<MeshFilter>().mesh = mesh;
    }

    void ComputeTangents(Vector3 v1, Vector3 v2, Vector3 v3, out Vector3 tangent, out Vector3 biTangent)
    {
        // First, compute a correct normal for the triangle using cross product
        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;

        // If the triangle has almost zero area, the normal will be small
        if (normal.magnitude <= 0.000001f)
        {
            tangent = Vector3.zero;
            biTangent = Vector3.zero;
            return;
        }
        normal.Normalize();

        Vector3 reference = Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.99f
                  ? Vector3.up
                  : Vector3.right;

        // Compute a tangent that is perpendicular to both the up vector and the normal
        //tangent = Vector3.Cross(Vector3.up, normal).normalized;
        tangent = Vector3.Cross(normal, reference).normalized;

        // Compute a bitangent that is perpendicular to both the normal and the tangent
        biTangent = Vector3.Cross(normal, tangent).normalized;
    }

    void ComputeTriangleUVs(Vector3 v1, Vector3 v2, Vector3 v3, ref Vector2 uv1, ref Vector2 uv2, ref Vector2 uv3, Vector3 tangent, Vector3 biTangent)
    {
        // Use the dot product onto unit vectors (the tangent and bitangent) for scalar projection. 
        // This gives the coordinates of each of the three points v1, v2 and v3, relative to the vector basis given by tangent & bitangent.
        // (See the 3D Math course for more details!)
        // Those coordinates will be used as the uvs.
        uv1 = new Vector2(Vector3.Dot(v1, tangent), Vector3.Dot(v1, biTangent)) / textureScaleFactor;
        uv2 = new Vector2(Vector3.Dot(v2, tangent), Vector3.Dot(v2, biTangent)) / textureScaleFactor;
        uv3 = new Vector2(Vector3.Dot(v3, tangent), Vector3.Dot(v3, biTangent)) / textureScaleFactor;
    }
}
