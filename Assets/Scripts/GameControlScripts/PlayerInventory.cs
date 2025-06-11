using System.Collections.Generic;
using UnityEngine;

public enum ItemType{
    Munny,
    Key,
    Soap
}

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int munny;
    public Dictionary<ItemType, int> Items;

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
        Items[item] += amount;
    }

    public void RemoveItem(ItemType item, int amount)
    {
        Items[item] -= amount;
    }

    public bool HasItem(ItemType item)
    {
        return (ItemAmount(item) > 0);
    }

    public int ItemAmount(ItemType item)
    {
        return Items[item];
    }
    
    
    private void DebugInventory()
    {
        Debug.Log("inventory");
        foreach (KeyValuePair<ItemType, int> kvp in Items)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}");
        }
    }
}
