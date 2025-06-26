using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshToTerrainHeightmap))]
public class MeshToTerrainHeightmapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshToTerrainHeightmap script = (MeshToTerrainHeightmap)target;

        GUILayout.Space(10);
        if (GUILayout.Button("🗺️ Convert Mesh to Terrain Heightmap"))
        {
            script.ConvertMeshToHeightmap();
        }

        if (!string.IsNullOrEmpty(script.lastResult))
        {
            EditorGUILayout.HelpBox(script.lastResult, MessageType.Info);
        }
    }
}
