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
    private TextMeshProUGUI inventoryText;
    private int currentUIIndex = 0;
    
    [SerializeField] GameObject inventory;
    [SerializeField] RawImage interactIcon;

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
        inventoryText = inventory.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(inventoryText);
    }

    private void UpdateInventory()
    {
        var inventoryString = "";

        for (int i = 0; i < player.inventory.Items.Count; i++)
        {
            if (i == currentUIIndex)
                inventoryString += ">";
            var kvp = player.inventory.Items.ElementAt(i);
            inventoryString += $"{kvp.Key}: {kvp.Value} \n";
        }


        inventoryText.text = inventoryString;
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
        inventory.SetActive(!inventory.activeSelf);
        player.controller.SetInventory(!inventory.activeSelf);
    }
}
