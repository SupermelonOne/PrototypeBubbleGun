using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueManager))]
public class Shop : MonoBehaviour
{
    private DialogueManager manager;
    private ShopUI shopUI;

    [HideInInspector] public Player player;

    private void OnEnable()
    {
        ShopEventBus.Subscribe<ShopEventBus.OnNavigateUI>(NavigateUI);
        ShopEventBus.Subscribe<ShopEventBus.OnNavigateUIReal>(NavigateUIReal);
        ShopEventBus.Subscribe<ShopEventBus.OnShopActivated>(OnShop);
    }

    private void OnDisable()
    {
        ShopEventBus.UnSubscribe<ShopEventBus.OnNavigateUI>(NavigateUI);
        ShopEventBus.UnSubscribe<ShopEventBus.OnNavigateUIReal>(NavigateUIReal);
        ShopEventBus.UnSubscribe<ShopEventBus.OnShopActivated>(OnShop);
    }

    private void Start()
    {
        manager = GetComponent<DialogueManager>();
        shopUI = GetComponentInChildren<ShopUI>();
        
        if(shopUI == null) Debug.LogError("ShopUI is null");

        shopUI.GenerateShopUI(manager.GetDialogueOptions(), manager);
    }

    public void OnShopInvoke(Player p)
    {
        ShopEventBus.Invoke(new ShopEventBus.OnShopActivated(p));
    }
    
    private void OnShop(ShopEventBus.OnShopActivated shopEvent)
    {
        player = shopEvent.player;
        player.controller.ToggleShopUI(true);
        shopUI.ActivateShopUI(player);
    }
    

    private void NavigateUI(ShopEventBus.OnNavigateUI shopEvent)
    {
        switch (shopEvent.inputType)
        {
            case InputTypes.Up:
                shopUI.OnMoveCursorUp();
                break;
            case InputTypes.Down:
                shopUI.OnMoveCursorDown();
                break;
            case InputTypes.Select:
                shopUI.OnSelectDialogueOption();
                break;
            case InputTypes.Back:
                shopUI.OnBack();
                break;
        }
    }

    private void NavigateUIReal(ShopEventBus.OnNavigateUIReal navigate)
    {
        Debug.Log("does it do anything????");

        switch (navigate.direction)
        {
            case 0:
                shopUI.OnMoveCursorUp();
                break;
            case 1:
                shopUI.OnMoveCursorDown();
                break;
            case 2:
                Debug.Log("does it work");
                shopUI.OnSelectDialogueOption();
                break;
            case 3:
                shopUI.OnBack();
                break;
        }
    }

    public bool Purchase(ItemType itemType, int amount, int cost)
    {
        
        if (PlayerInventory.Instance.ItemAmount(ItemType.Munny) >= cost)
        {
            Debug.Log("You have " + amount + " " + itemType + " in your inventory");
            PlayerInventory.Instance.BuyItem(itemType, amount, cost);
           
            return true;
        }
        
        

        return false;
    }
}
