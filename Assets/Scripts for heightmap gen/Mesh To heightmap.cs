using UnityEngine;

[ExecuteInEditMode]
public class MeshToTerrainHeightmap : MonoBehaviour
{
    public MeshFilter sourceMeshFilter;
    public Terrain targetTerrain;

    [Range(33, 4097)]
    public int heightmapResolution = 513;

    [HideInInspector]
    public string lastResult;

    public void ConvertMeshToHeightmap()
    {
        if (sourceMeshFilter == null || targetTerrain == null)
        {
            lastResult = "❌ Please assign both MeshFilter and Terrain.";
            return;
        }

        Mesh mesh = sourceMeshFilter.sharedMesh;
        if (!mesh.isReadable)
        {
            lastResult = "❌ Mesh is not readable. Enable 'Read/Write' in import settings.";
            return;
        }

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Bounds bounds = mesh.bounds;

        // Set resolution
        int res = heightmapResolution;
        float[,] heights = new float[res, res];

        float minX = bounds.min.x;
        float minZ = bounds.min.z;
        float sizeX = bounds.size.x;
        float sizeZ = bounds.size.z;
        float sizeY = bounds.size.y;

        // Fill heightmap by rasterizing triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            Vector2 hv0 = new Vector2((v0.x - minX) / sizeX, (v0.z - minZ) / sizeZ);
            Vector2 hv1 = new Vector2((v1.x - minX) / sizeX, (v1.z - minZ) / sizeZ);
            Vector2 hv2 = new Vector2((v2.x - minX) / sizeX, (v2.z - minZ) / sizeZ);

            float h0 = (v0.y - bounds.min.y) / sizeY;
            float h1 = (v1.y - bounds.min.y) / sizeY;
            float h2 = (v2.y - bounds.min.y) / sizeY;

            int minXPixel = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(hv0.x, hv1.x, hv2.x) * res), 0, res - 1);
            int maxXPixel = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(hv0.x, hv1.x, hv2.x) * res), 0, res - 1);
            int minZPixel = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(hv0.y, hv1.y, hv2.y) * res), 0, res - 1);
            int maxZPixel = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(hv0.y, hv1.y, hv2.y) * res), 0, res - 1);

            for (int z = minZPixel; z <= maxZPixel; z++)
            {
                for (int x = minXPixel; x <= maxXPixel; x++)
                {
                    Vector2 p = new Vector2((float)x / (res - 1), (float)z / (res - 1));
                    if (PointInTriangle(p, hv0, hv1, hv2, out Vector3 bary))
                    {
                        float height = h0 * bary.x + h1 * bary.y + h2 * bary.z;
                        heights[z, x] = Mathf.Max(heights[z, x], height);
                    }
                }
            }
        }

        // Create a new terrain data instance
        TerrainData terrainData = targetTerrain.terrainData;
        terrainData.heightmapResolution = res;
        terrainData.size = new Vector3(sizeX, sizeY, sizeZ);
        terrainData.SetHeights(0, 0, heights);

        // Position terrain to match mesh in world space
        Vector3 worldMin = sourceMeshFilter.transform.TransformPoint(bounds.min);
        targetTerrain.transform.position = new Vector3(worldMin.x, bounds.min.y, worldMin.z);

        lastResult = "✅ Mesh converted to terrain heightmap with 1:1 size.";
    }

    private bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out Vector3 bary)
    {
        Vector2 v0 = b - a;
        Vector2 v1 = c - a;
        Vector2 v2 = p - a;

        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);

        float denom = d00 * d11 - d01 * d01;
        if (Mathf.Abs(denom) < 1e-6f)
        {
            bary = Vector3.zero;
            return false;
        }

        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        bary = new Vector3(u, v, w);
        return u >= 0 && v >= 0 && w >= 0;
    }
}
