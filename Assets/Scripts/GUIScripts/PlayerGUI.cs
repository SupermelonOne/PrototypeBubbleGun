using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    private Player player;
    private Canvas canvas;
    
    [SerializeField] TextMeshProUGUI inventoryText;
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

        UpdateInventory();
        Initialize();
        
        player.inventory.OnChange += UpdateInventory;
    }
    
    private void OnDestroy()
    {
        if (player != null)
            player.inventory.OnChange -= UpdateInventory;
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
        var inventoryString = "";

        foreach (var kvp in player.inventory.Items)
            inventoryString += $"{kvp.Key}: {kvp.Value} \n";
        
        
        inventoryText.text = inventoryString;
    }

    public void OnInteract(bool isOn)
    {
        interactIcon.enabled = isOn;
    }
}
