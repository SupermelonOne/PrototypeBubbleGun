using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BridgeScript))]
public class BridgeEditor : Editor
{
    private SerializedProperty plankMaterials;

    void OnEnable()
    {
        plankMaterials = serializedObject.FindProperty("plankMaterials");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        BridgeScript bridge = (BridgeScript)target;

        EditorGUI.BeginChangeCheck();

        GUILayout.Label("Bridge Location", EditorStyles.boldLabel);
        bridge.bridgeStartObject = (GameObject)EditorGUILayout.ObjectField("Bridge Start", bridge.bridgeStartObject, typeof(GameObject), true);
        bridge.bridgeEndObject = (GameObject)EditorGUILayout.ObjectField("Bridge End", bridge.bridgeEndObject, typeof(GameObject), true);
        bridge.emptySpace = EditorGUILayout.FloatField("Empty Space", bridge.emptySpace);
        bridge.fallDist = EditorGUILayout.FloatField("Fall Distance", bridge.fallDist);
        bridge.maxCurveHeight = EditorGUILayout.FloatField("Max Height", bridge.maxCurveHeight);

        GUILayout.Label("Bridge looks", EditorStyles.boldLabel);
        bridge.plankPrefab = (GameObject)EditorGUILayout.ObjectField("Plank Prefab", bridge.plankPrefab, typeof(GameObject), false);
        EditorGUILayout.PropertyField(plankMaterials, new GUIContent("Plank Materials"), true);

        GUILayout.Label("Bridge Sound Effects", EditorStyles.boldLabel);
        bridge.soundEffect = (AudioClip)EditorGUILayout.ObjectField("Sound Effect", bridge.soundEffect, typeof(AudioClip), false);

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(bridge);

        if (GUILayout.Button("Build Bridge"))
            bridge.OnBuildBridge();

        if (GUILayout.Button("Build Bridge with delay"))
            bridge.OnBuildBridgeWithDelay();

        if (GUILayout.Button("Clear Bridge"))
            bridge.ClearBridge();
        
        serializedObject.ApplyModifiedProperties();
    }
}