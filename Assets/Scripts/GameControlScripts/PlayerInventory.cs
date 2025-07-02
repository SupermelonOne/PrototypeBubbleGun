using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
    Munny,
    Key,
    Soap
}

public delegate void Change();

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int munny;
    public Dictionary<ItemType, int> Items;
    public event Change OnChange;


    private void OnEnable()
    {
        MonsterEventBus.Subscribe<MonsterEventBus.DirtCleaned>(AddItem);
    }

    private void OnDisable()
    {
        MonsterEventBus.UnSubscribe<MonsterEventBus.DirtCleaned>(AddItem);
    }



    private void Start()
    {
        Items = new Dictionary<ItemType, int>();
        Items.Add(ItemType.Munny, munny);
        Items.Add(ItemType.Key, 0);
        Items.Add(ItemType.Soap, 0);
    }

    public void BuyItem(ItemType item, int amount, int cost)
    {
        AddItem(item, amount);
        RemoveItem(ItemType.Munny, cost);
    }

    public void AddItem(ItemType item, int amount)
    {
        var a = Items[item] + amount;
        UpdateItems(item, a);
    }
    public void AddItem(MonsterEventBus.DirtCleaned dirtCleaned)
    {
        var a = Items[dirtCleaned.type] + dirtCleaned.amount;
        UpdateItems(dirtCleaned.type, a);
    }

    public void RemoveItem(ItemType item, int amount)
    {
        var a = Items[item] - amount;
        UpdateItems(item, a);
    }

    private void UpdateItems(ItemType item, int amount)
    {
        Items[item] = amount;
        OnChange?.Invoke();
    }

    public bool HasItem(ItemType item)
    {
        return (ItemAmount(item) > 0);
    }

    public int ItemAmount(ItemType item)
    {
        return Items[item];
    }
}
