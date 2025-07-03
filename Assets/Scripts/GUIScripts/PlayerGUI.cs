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
    private Vector2Int currentUIIndex = new Vector2Int(0,0);

    [SerializeField] private GameObject tutorial;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject inventoryText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private RawImage itemIcon;
    [SerializeField] private RawImage interactIcon;
    [SerializeField] private GameObject itemTextPrefab;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private int rowCount = 2;

    public void AssignPlayer(Player p)
    {
        player = p;
    }

    private void Start()
    {
        if (player?.inventory == null)
        {
            var message = player == null ? "Player is null" : "Player Inventory is null";
            Debug.LogError(message);
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

        var rows = new List<Transform>();
        RectTransform inventoryTextRect = inventoryText.GetComponent<RectTransform>();

        for (int i = 0; i < rowCount; i++)
        {
            var row = new GameObject($"Row{i + 1}", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            
            row.transform.SetParent(inventoryTextRect, false);

            var rowRect = row.GetComponent<RectTransform>();
            var step = 1f / rowCount;
            var anchorMinY = 1f - step * (i + 1);
            var anchorMaxY = 1f - step * i;

            rowRect.anchorMin = new Vector2(0f, anchorMinY);
            rowRect.anchorMax = new Vector2(1f, anchorMaxY);
            rowRect.offsetMin = Vector2.zero;
            rowRect.offsetMax = Vector2.zero;
            rows.Add(row.transform);
        }


        

        foreach (var kvp in items)
        {
            // Create new TextMeshProUGUI object
            GameObject textObj = Instantiate(itemTextPrefab, inventoryText.transform);
            textObj.name = "Item" + kvp.Key;
            textObj.transform.localScale = Vector3.one;
            
            int rowIndex = Mathf.FloorToInt((float)index / items.Count * rowCount);
            rowIndex = Mathf.Clamp(rowIndex, 0, rowCount - 1);
            textObj.transform.SetParent(rows[rowIndex], false);
            
            var tmp = textObj.GetComponentInChildren<TextMeshProUGUI>();
            tmp.fontSize = 34;
            tmp.text = $"{kvp.Key}: {kvp.Value.amount}";
            tmp.color = Color.white;
            
            var img = textObj.GetComponentInChildren<RawImage>();
            img.texture = kvp.Value.icon;

            // Highlight current selection
            int column = rows[rowIndex].childCount; 
            textObj.transform.SetParent(rows[rowIndex], false);

            if (currentUIIndex.x+1 == column && currentUIIndex.y == rowIndex)
            {
                tmp.text = $"> {kvp.Key}: {kvp.Value.amount}";
                tmp.color = Color.white;

                if (player.inventory.Items.TryGetValue(kvp.Key, out var value))
                {
                    itemDescriptionText.text = value.description;
                    itemNameText.text = kvp.Key.ToString();
                    
                    itemIcon.texture = kvp.Value.icon;
                }
            }

            index++;
        }
    }

    
    private void OnMoveCursor(InventoryEventBus.OnNavigateUI navigateUI)
    {
        var totalItems = player.inventory.Items.Count;
        var columns = totalItems / rowCount;

        if (navigateUI.inputType == InputTypes.Left && currentUIIndex.x > 0)
            currentUIIndex.x--;

        if (navigateUI.inputType == InputTypes.Right && currentUIIndex.x < columns - 1)
            currentUIIndex.x++;

        if (navigateUI.inputType == InputTypes.Up && currentUIIndex.y > 0)
            currentUIIndex.y--;

        if (navigateUI.inputType == InputTypes.Down && currentUIIndex.y < rowCount - 1)
            currentUIIndex.y++;

        UpdateInventory();
    }


    public void OnInteract(bool isOn)
    {
        interactIcon.enabled = isOn;
    }

    public void ToggleUI()
    {
        if (tutorial.activeSelf)
        {
            player.controller.SetInventory(false);
            crosshair.SetActive(true);
            inventory.SetActive(false);
            tutorial.SetActive(false);
            return;
        }
        player.controller.SetInventory(inventory.activeSelf);
        crosshair.SetActive(inventory.activeSelf);
        inventory.SetActive(!inventory.activeSelf);
    }
}
