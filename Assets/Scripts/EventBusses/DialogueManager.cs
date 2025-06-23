using System.Collections.Generic;
using UnityEngine;


public class DialogueManager : MonoBehaviour
{
    [SerializeReference] public List<DialogueOption> dialogueOptions = new();
    
    private Dictionary<string, string> dialogueByName = new();
    private Dictionary<int, string> dialogueByID = new();
    
    [HideInInspector] public Shop shop;
    
    void Awake()
    {
        shop = GetComponent<Shop>();
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

    public void HandleBuyOption(BuyOption option)
    {
        shop.Purchase(option.item, option.amount, option.price);
    }
    
    public List<DialogueOption> GetDialogueOptions() => dialogueOptions;

}