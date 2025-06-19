using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab; // Panel prefab with dialogue text + container for choices
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI textOptionPrefab;
    
    private Canvas canvas; // Container prefab with Canvas
    private List<GameObject> panels;
    private Dictionary<int, GameObject> panelDictionary = new Dictionary<int, GameObject>();

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void ActivateShopUI()
    {
        canvas.enabled = true;
    }
    public void GenerateShopUI(List<DialogueOption> dialogueOptions)
    {
        foreach (var dialogueOption in dialogueOptions)
        {
            GeneratePanel(dialogueOption);
        }
    }

    void GeneratePanel(DialogueOption dialogueOption)
    {
        Debug.Log($"Generating panel {dialogueOption.dialogueName}");
        GameObject panel = Instantiate(panelPrefab, canvas.transform);
        panel.name = dialogueOption.dialogueName + "Panel";
        
        Debug.Log($"dialogue is: {dialogueOption.dialogue}");
        var dialogueText = dialogueBox.GetComponent<TextMeshProUGUI>();
        dialogueText.text = dialogueOption.dialogue;
        
        panelDictionary.Add(dialogueOption.ID, panel);
        panel.SetActive(false);

        // Get container transform for choice texts
        Transform choicesContainer = panel.transform;

        // Add a TextMeshProUGUI component for each choice dynamically inside container
        foreach (var choice in dialogueOption.options)
        {
            var choiceGO = new GameObject(choice.dialogueName, typeof(RectTransform));
            choiceGO.transform.SetParent(choicesContainer, false);

            var choiceText = Instantiate(textOptionPrefab, choicesContainer);
            choiceText.text = choice.dialogueName; // Or whatever the choice text is
        }
    }

    public void SetPanel(int id, bool active)
    {
        var panel = panelDictionary[id];
        panel.SetActive(active);
    }
}