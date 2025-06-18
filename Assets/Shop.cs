using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueManager))]
[RequireComponent(typeof(ShopUI))]
public class Shop : MonoBehaviour
{
    private DialogueManager manager;
    private ShopUI shopUI;
    private void Start()
    {
        ShopEventBus.Subscribe<ShopEventBus.OnShopActivated>(OnShop);
        manager = GetComponent<DialogueManager>();
        shopUI = GetComponent<ShopUI>();
    }

    public void OnShopInvoke()
    {
        ShopEventBus.Invoke(new ShopEventBus.OnShopActivated());
    }
    
    private void OnShop(ShopEventBus.OnShopActivated shopEvent)
    {
        shopUI.GenerateShopUI(manager.GetDialogueOptions());
    }
}
