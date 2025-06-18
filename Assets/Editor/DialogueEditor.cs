#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Events;

[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor
{
    private const int MAX_DEPTH = 10;
    private SerializedProperty dialoguesProp;
    private DialogueManager manager;

    private void OnEnable()
    {
        // Cache the target and property for efficiency
        manager = (DialogueManager)target;
        dialoguesProp = serializedObject.FindProperty("dialogueOptions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Always start with this

        // --- Default UI for adding/removing root elements ---
        EditorGUILayout.LabelField("Root Dialogues (Managed)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(dialoguesProp);
        
        // Find and replace any null elements created by the default UI
        for (int i = 0; i < dialoguesProp.arraySize; i++)
        {
            SerializedProperty element = dialoguesProp.GetArrayElementAtIndex(i);
            if (element.managedReferenceValue == null)
            {
                element.managedReferenceValue = new DialogueOption { dialogueName = "New Root Dialogue" };
            }
        }
        
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Assign All IDs"))
        {
            Undo.RecordObject(manager, "Assign Dialogue IDs");
            int id = 0;
            DialogueManager.AssignIDs(manager.dialogueOptions);
            EditorUtility.SetDirty(manager);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Dialogue Tree Editor", EditorStyles.boldLabel);

        // --- Custom UI for drawing the tree ---
        // We iterate backwards to prevent issues when deleting
        for (int i = dialoguesProp.arraySize - 1; i >= 0; i--)
        {
            SerializedProperty element = dialoguesProp.GetArrayElementAtIndex(i);
            DialogueOption option = element.managedReferenceValue as DialogueOption;
            if (option != null)
            {
                // We pass the manager and the list index to the drawing function
                DrawDialogueOption(option, 0, i);
            }
        }

        if (GUILayout.Button("Add Root Dialogue"))
        {
            // This is a safe way to add
            Undo.RecordObject(manager, "Add Root Dialogue");
            manager.dialogueOptions.Add(new DialogueOption { dialogueName = "New Root Dialogue" });
            EditorUtility.SetDirty(manager);
        }

        serializedObject.ApplyModifiedProperties(); // Always end with this
    }

    void DrawDialogueOption(DialogueOption option, int depth, int listIndex)
    {
        if (depth > MAX_DEPTH)
        {
            EditorGUILayout.HelpBox("Maximum dialogue depth reached.", MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginVertical("box");

        // --- Header and Deletion ---
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"ID: {option.ID} | Depth: {depth}", EditorStyles.boldLabel);
        
        // This is the FIX: We check for deletion at the root level (depth == 0)
        if (depth == 0)
        {
            GUI.backgroundColor = new Color(1, 0.6f, 0.6f); // Make delete button red
            if (GUILayout.Button("Delete Root", GUILayout.Width(100)))
            {
                // THE FIX IS HERE: Modify the actual list, not the SerializedProperty
                Undo.RecordObject(manager, "Delete Root Dialogue");
                manager.dialogueOptions.RemoveAt(listIndex);
                GUIUtility.ExitGUI(); // Exit GUI to prevent errors from drawing a deleted element
            }
            GUI.backgroundColor = Color.white; // Reset color
        }
        EditorGUILayout.EndHorizontal();
        
        // --- Dialogue Fields ---
        option.dialogueName = EditorGUILayout.TextField("Name", option.dialogueName);
        
        EditorGUILayout.LabelField("Dialogue Text");
        option.dialogue = EditorGUILayout.TextArea(option.dialogue, GUILayout.MinHeight(60));

        // --- Child Options ---
        EditorGUILayout.LabelField("Responses / Next Dialogues", EditorStyles.boldLabel);

        int toRemove = -1;
        for (int i = 0; i < option.options.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            // For child options, we don't need the manager, just the parent option's list
            DrawDialogueOption(option.options[i], depth + 1, i);
            
            GUI.backgroundColor = new Color(1, 0.8f, 0.8f);
            if (GUILayout.Button("Remove This Response"))
            {
                toRemove = i; // Mark for removal
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        if (toRemove >= 0)
        {
            Undo.RecordObject(manager, "Remove Dialogue Response");
            option.options.RemoveAt(toRemove);
        }

        if (GUILayout.Button("Add Response"))
        {
            Undo.RecordObject(manager, "Add Dialogue Response");
            option.options.Add(new DialogueOption() { dialogueName = "New Response" });
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
    }
}
#endif