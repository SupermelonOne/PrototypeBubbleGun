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
        manager = (DialogueManager)target;
        dialoguesProp = serializedObject.FindProperty("dialogueOptions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Root Dialogues (Managed)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(dialoguesProp);
        
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
            DialogueManager.AssignIDs(manager.dialogueOptions);
            EditorUtility.SetDirty(manager);
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Dialogue Tree Editor", EditorStyles.boldLabel);

        for (int i = dialoguesProp.arraySize - 1; i >= 0; i--)
        {
            SerializedProperty elementProp = dialoguesProp.GetArrayElementAtIndex(i);
            DialogueOption option = elementProp.managedReferenceValue as DialogueOption;
            if (option != null)
            {
                // Pass the SerializedProperty along with the object
                DrawDialogueOption(elementProp, option, 0, i);
            }
        }

        if (GUILayout.Button("Add Root Dialogue"))
        {
            Undo.RecordObject(manager, "Add Root Dialogue");
            manager.dialogueOptions.Add(new DialogueOption { dialogueName = "New Root Dialogue" });
            EditorUtility.SetDirty(manager);
        }

        serializedObject.ApplyModifiedProperties();
    }

    // Note the new 'optionProp' parameter
    void DrawDialogueOption(SerializedProperty optionProp, DialogueOption option, int depth, int listIndex)
    {
        if (depth > MAX_DEPTH)
        {
            EditorGUILayout.HelpBox("Maximum dialogue depth reached.", MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"ID: {option.ID} | Depth: {depth}", EditorStyles.boldLabel);
        
        if (depth == 0)
        {
            GUI.backgroundColor = new Color(1, 0.6f, 0.6f);
            if (GUILayout.Button("Delete Root", GUILayout.Width(100)))
            {
                Undo.RecordObject(manager, "Delete Root Dialogue");
                manager.dialogueOptions.RemoveAt(listIndex);
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUILayout.EndHorizontal();
        
        option.dialogueName = EditorGUILayout.TextField("Name", option.dialogueName);
        
        EditorGUILayout.LabelField("Dialogue Text");
        option.dialogue = EditorGUILayout.TextArea(option.dialogue, GUILayout.MinHeight(60));
        
        
        EditorGUILayout.PropertyField(optionProp.FindPropertyRelative("onSelected"));

        EditorGUILayout.LabelField("Responses / Next Dialogues", EditorStyles.boldLabel);

        // We need the 'options' property for the recursive call
        SerializedProperty childrenProp = optionProp.FindPropertyRelative("options");
        int toRemove = -1;

        for (int i = 0; i < option.options.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");

            // Pass the child's property and object to the recursive call
            DrawDialogueOption(childrenProp.GetArrayElementAtIndex(i), option.options[i], depth + 1, i);
            
            GUI.backgroundColor = new Color(1, 0.8f, 0.8f);
            if (GUILayout.Button("Remove This Response"))
            {
                toRemove = i;
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

//it's a lil bit cluttered, but overall it looks nice. The ending is a bit abrupt though. -Elin