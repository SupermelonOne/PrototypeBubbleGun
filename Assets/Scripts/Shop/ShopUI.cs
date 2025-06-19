using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private string initialText = "";
    [SerializeField] private GameObject panelPrefab; // Panel prefab with dialogue text + container for choices
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textOptionPrefab;
    
    private Canvas canvas; // Container prefab with Canvas
    private GameObject panel;
    private DialogueOption currentDialogueOption;
    private Stack<DialogueOption> dialogueHistory = new Stack<DialogueOption>();
    private int currentDialogueOptionIndex = 0;
    private List<TextMeshProUGUI> dialogueBoxList = new List<TextMeshProUGUI>();
    private Dictionary<DialogueOption, TextMeshProUGUI> dialogueBoxDict = new Dictionary<DialogueOption, TextMeshProUGUI>();

    //TODO: fix it so 2 players can use it yknow
    public Player player;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void ActivateShopUI(Player p)
    {
        canvas.enabled = true;
        player = p;
    }
    public void GenerateShopUI(List<DialogueOption> dialogueOptions)
    {
        panel = Instantiate(panelPrefab, canvas.transform);
        panel.name = "Panel";
        
        DialogueOption option = new DialogueOption();
        option.dialogue = initialText;
        foreach (var dialogueOption in dialogueOptions)
        {
            option.options.Add(dialogueOption);
        }
        GeneratePanel(option);
    }

    void GeneratePanel(DialogueOption dialogueOption)
    {
        currentDialogueOption = dialogueOption;

        var dialogueText = dialogueBox.GetComponent<TextMeshProUGUI>();
        dialogueText.text = dialogueOption.dialogue;
        
        Transform choicesContainer = panel.transform;
        
        if (dialogueOption.options.Count <= 0)
            return;

        foreach (var choice in dialogueOption.options)
        {
            var choiceText = Instantiate(textOptionPrefab, choicesContainer);
            dialogueBoxList.Add(choiceText);
            dialogueBoxDict.Add(choice, choiceText);
            choiceText.text = choice.dialogueName; 
        }

        SetCursor();
    }

    public void OnMoveCursorUp()
    {
        if (currentDialogueOptionIndex - 1 >= 0)
            currentDialogueOptionIndex--;
        
        
        SetCursor();
    }

    public void OnMoveCursorDown()
    {
        if (currentDialogueOptionIndex + 1 < currentDialogueOption.options.Count)
            currentDialogueOptionIndex++;
        
        SetCursor();
    }

    private void ResetNames()
    {
        foreach (var kvp in dialogueBoxDict)
        {
            kvp.Value.text = kvp.Key.dialogueName;
        }
    }

    private void SetCursor()
    {
        ResetNames();
        var d = currentDialogueOption.options[currentDialogueOptionIndex];
        var dialogueText = dialogueBoxDict[d];
        dialogueText.text = "> " + d.dialogueName;
    }

    public void OnSelectDialogueOption()
    {
        OnOptionSelected(currentDialogueOption.options[currentDialogueOptionIndex], currentDialogueOption);
    }

    public void OnBack()
    {
        if (dialogueHistory.Count > 0)
        {
            DialogueOption previous = dialogueHistory.Pop();
            DestroyPanel();
            GeneratePanel(previous);
        }
        else
        {
            DeactivateUI(player);
        }
    }


    private void DeactivateUI(Player player)
    {
        player.controller.ToggleShopUI(false);
        canvas.enabled = false;
    }
    
    
    void DestroyPanel()
    {
        currentDialogueOptionIndex = 0;
        foreach (var d in dialogueBoxList)
        {
            Destroy(d.gameObject);
        }
        dialogueBoxList.Clear();
        dialogueBoxDict.Clear();
    }

    void OnOptionSelected(DialogueOption dialogueOption, DialogueOption from = null)
    {
        if (from != null)
            dialogueHistory.Push(from);
        DestroyPanel();
        GeneratePanel(dialogueOption);
    }

}