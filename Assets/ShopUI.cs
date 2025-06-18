using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject canvas; // Container prefab with Canvas
    [SerializeField] private GameObject panelPrefab; // Panel prefab with dialogue text + container for choices
    [SerializeField] private GameObject dialogueBox;
    
    public void GenerateShopUI(List<DialogueOption> dialogueOptions)
    {
        foreach (var dialogueOption in dialogueOptions)
        {
            GeneratePanel(dialogueOption);
        }
    }

    void GeneratePanel(DialogueOption dialogueOption)
    {
        GameObject panel = Instantiate(panelPrefab, canvas.transform);

        var dialogueText = dialogueBox.GetComponent<TextMeshProUGUI>();
        dialogueText.text = dialogueOption.dialogue;

        // Get container transform for choice texts
        Transform choicesContainer = panel.transform;

        // Add a TextMeshProUGUI component for each choice dynamically inside container
        foreach (var choice in dialogueOption.options)
        {
            var choiceGO = new GameObject(choice.dialogueName, typeof(RectTransform));
            choiceGO.transform.SetParent(choicesContainer, false);

            var choiceText = choiceGO.AddComponent<TextMeshProUGUI>();
            choiceText.text = choice.dialogueName; // Or whatever the choice text is
            choiceText.fontSize = 18;
            choiceText.color = Color.yellow;
        }
    }
}