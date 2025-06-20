using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueManager))]
public class Shop : MonoBehaviour
{
    private DialogueManager manager;
    private ShopUI shopUI;

    private void OnEnable()
    {
        ShopEventBus.Subscribe<ShopEventBus.OnNavigateUI>(NavigateUI);
    }

    private void OnDisable()
    {
        ShopEventBus.UnSubscribe<ShopEventBus.OnNavigateUI>(NavigateUI);
    }

    private void Start()
    {
        ShopEventBus.Subscribe<ShopEventBus.OnShopActivated>(OnShop);
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
        shopEvent.player.controller.ToggleShopUI(true);
        shopUI.ActivateShopUI(shopEvent.player);
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

    public void Purchase(ItemType itemType, int amount, int cost)
    {
        //TODO: add dialogue if not enough money
        if (shopUI.player.inventory.ItemAmount(ItemType.Munny) >= cost)
        {
            shopUI.player.inventory.BuyItem(itemType, amount, cost);
            Debug.Log("Purchased Item: " + itemType + " with amount: " + amount + " to " + cost);
        }
    }
}
//Idk I just don't feel this one Elin