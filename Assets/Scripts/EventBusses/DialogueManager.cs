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

        void Assign(Option option, int currentDepth)
        {
            if (currentDepth > 10)
            {
                Debug.LogError($"Dialogue '{option.name}' exceeds max depth of 10.");
                return;
            }
            
            
            option.id = idCounter++;
            option.layer = currentDepth;

            if (option is DialogueOption)
            {
                var op = (DialogueOption)option;
                foreach (var child in op.options)
                {
                    Assign(child, currentDepth + 1);
                }
            }
        }
        
        foreach (var dialogue in dialogues)
            Assign(dialogue, 0);        
    }
//the dialogue is lacking -Elin
    private void UpdateDictionaries(List<DialogueOption> dialogues)
    { 
        void Assign(Option option, int currentDepth)
        {
            dialogueByName[option.name] = option.description;
            dialogueByID[option.id] = option.description;
            option.layer = currentDepth;

            if (option is DialogueOption)
            {
                var op = (DialogueOption)option;
                foreach (var child in op.options)
                {
                    Assign(child, currentDepth + 1);
                }
            }
        }

        foreach (var dialogue in dialogues)
            Assign(dialogue, 0);
        
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

    public void HandleBuyOption(BuyOption option)
    {
        Debug.Log($"Buying {option.amount}x {option.item} for {option.price} Rupees.");
    }
    
    public List<DialogueOption> GetDialogueOptions() => dialogueOptions;

}