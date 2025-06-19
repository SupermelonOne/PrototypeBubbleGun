using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueManager))]
public class Shop : MonoBehaviour
{
    private DialogueManager manager;
    private ShopUI shopUI;
    private void Start()
    {
        ShopEventBus.Subscribe<ShopEventBus.OnShopActivated>(OnShop);
        manager = GetComponent<DialogueManager>();
        shopUI = GetComponentInChildren<ShopUI>();
        
        if(shopUI == null) Debug.LogError("ShopUI is null");

        shopUI.GenerateShopUI(manager.GetDialogueOptions());
        
    }

    public void OnShopInvoke(Player p)
    {
        ShopEventBus.Invoke(new ShopEventBus.OnShopActivated());
    }
    
    private void OnShop(ShopEventBus.OnShopActivated shopEvent)
    {
        shopUI.ActivateShopUI();
        shopUI.SetPanel(0, true);
    }
}
