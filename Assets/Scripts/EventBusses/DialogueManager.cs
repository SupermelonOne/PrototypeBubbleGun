using System.Collections.Generic;
using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    [SerializeReference] public List<DialogueOption> dialogueOptions = new();
    
    private Dictionary<string, string> dialogueByName = new();
    private Dictionary<int, string> dialogueByID = new();
    
    void Awake()
    {
        AssignIDs(dialogueOptions);
        UpdateDictionaries(dialogueOptions);
    }

    public static void AssignIDs(List<DialogueOption> dialogues)
    {
        int idCounter = 0;

        void Assign(DialogueOption option, int currentDepth)
        {
            if (currentDepth > 10)
            {
                Debug.LogError($"Dialogue '{option.dialogueName}' exceeds max depth of 10.");
                return;
            }
            
            
            option.ID = idCounter++;
            option.layer = currentDepth;

            foreach (var child in option.options)
            {
                Assign(child, currentDepth + 1);
            }
        }

        foreach (var dialogue in dialogues)
        {
            Assign(dialogue, 0);
        }
    }

    private void UpdateDictionaries(List<DialogueOption> dialogues)
    { 
        void Assign(DialogueOption option, int currentDepth)
        {
            dialogueByName[option.dialogueName] = option.dialogue;
            dialogueByID[option.ID] = option.dialogue;
            option.layer = currentDepth;

            foreach (var child in option.options)
            {
                Assign(child, currentDepth + 1);
            }
        }

        foreach (var dialogue in dialogues)
        {
            Assign(dialogue, 0);
        }
    }

    public string GetDialogue(int ID)
    {
        if (dialogueByID.ContainsKey(ID))
            return dialogueByID[ID];

        return "You broke the game genius...";
    }
    
    public string GetDialogue(string dialogueName)
    {
        if (dialogueByName.ContainsKey(dialogueName))
            return dialogueByName[dialogueName];

        return "You broke the game genius...";
    }
    
    public List<DialogueOption> GetDialogueOptions() => dialogueOptions;

}