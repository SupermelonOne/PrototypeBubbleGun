using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType{
    Munny,
    Key,
    Soap
}

[System.Serializable]
public struct ItemStruct
{
    public ItemType type;
    public string description;
    public Sprite icon;
}

public delegate void Change();

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }
    
    [SerializeField] private int munny;

    [SerializeField] private List<ItemStruct> itemDescriptions;
    public Dictionary<ItemType, string> itemDescriptionsDictionary = new Dictionary<ItemType, string>();
    public Dictionary<ItemType, int> Items;
    public event Change OnChange;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    private void Start()
    {
        Items = new Dictionary<ItemType, int>();
        Items.Add(ItemType.Munny, munny);
        Items.Add(ItemType.Key, 0);
        Items.Add(ItemType.Soap, 0);

        foreach (ItemStruct item in itemDescriptions)
        {
            if (!itemDescriptionsDictionary.ContainsKey(item.type))
                itemDescriptionsDictionary.Add(item.type, item.description);
        }
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
