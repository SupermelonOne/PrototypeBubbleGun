using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    private Player player;
    private Canvas canvas;
    private int currentUIIndex = 0;
    
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private RawImage interactIcon;
    [SerializeField] private GameObject itemTextPrefab;
    [SerializeField] private GameObject crosshair;

    public void AssignPlayer(Player p)
    {
        player = p;
    }

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player is null");
            return;
        }
        

        Initialize();
        UpdateInventory();

        InventoryEventBus.Subscribe<InventoryEventBus.OnNavigateUI>(OnMoveCursor);
        player.inventory.OnChange += UpdateInventory;
    }
    
    private void OnDestroy()
    {
        if (player != null)
            player.inventory.OnChange -= UpdateInventory;
        InventoryEventBus.UnSubscribe<InventoryEventBus.OnNavigateUI>(OnMoveCursor);

    }

    private void Initialize()
    {
        canvas = GetComponent<Canvas>();

        canvas.worldCamera = player.controller.playerCamera;
        canvas.planeDistance = 1f;
        interactIcon.enabled = false;
    }

    private void UpdateInventory()
    {
        // Clear old children
        foreach (Transform child in inventoryText.transform)
        {
            Destroy(child.gameObject);
        }

        var items = player.inventory.Items;
        int index = 0;

        foreach (var kvp in items)
        {
            // Create new TextMeshProUGUI object
            GameObject textObj = Instantiate(itemTextPrefab, inventoryText.transform);
            textObj.name = "Item" + kvp.Key;
            textObj.transform.localScale = Vector3.one;

            var tmp = textObj.GetComponentInChildren<TextMeshProUGUI>();
            tmp.fontSize = 34;
            tmp.text = $"{kvp.Key}: {kvp.Value}";
            tmp.color = Color.black;

            // Highlight current selection
            if (index == currentUIIndex)
            {
                tmp.text = $"> {kvp.Key}: {kvp.Value}";
                tmp.color = Color.black;

                if (player.inventory.itemDescriptionsDictionary.TryGetValue(kvp.Key, out var value))
                    itemDescriptionText.text = value;
            }

            index++;
        }
    }

    
    public void OnMoveCursor(InventoryEventBus.OnNavigateUI navigateUI)
    {
        
        if (currentUIIndex - 1 >= 0 && navigateUI.inputType == InputTypes.Up)
            currentUIIndex--;
        
        if (currentUIIndex + 1 < player.inventory.Items.Count && navigateUI.inputType == InputTypes.Down)
            currentUIIndex++;
        
        UpdateInventory();
    }

    public void OnInteract(bool isOn)
    {
        interactIcon.enabled = isOn;
    }

    public void ToggleUI()
    {
        crosshair.SetActive(inventory.activeSelf);
        inventory.SetActive(!inventory.activeSelf);
        player.controller.SetInventory(!inventory.activeSelf);
    }
}
