using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType{
    Munny,
    Key,
    Soap,
    Candy
}

[System.Serializable]
public struct ItemStruct
{
    public ItemType type;
    public string description;
    public Texture icon;
    public int amount;

    public void SetAmount(int setAmount)
    {
        amount = setAmount;
    }
}

public delegate void Change();

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }
    
    [SerializeField] private int munny;

    [SerializeField] private List<ItemStruct> itemDescriptions;
    public Dictionary<ItemType, ItemStruct> Items = new Dictionary<ItemType, ItemStruct>();
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
        foreach (ItemStruct item in itemDescriptions)
        {
            Items.TryAdd(item.type, item);
        }
    }

    public void BuyItem(ItemType item, int amount, int cost)
    {
        AddItem(item, amount);
        RemoveItem(ItemType.Munny, cost);
    }

    public void AddItem(ItemType item, int amount)
    {
        var a = Items[item].amount + amount;
        UpdateItems(item, a);
    }

    public void RemoveItem(ItemType item, int amount)
    {
        var a = Items[item].amount - amount;
        UpdateItems(item, a);
    }

    private void UpdateItems(ItemType item, int amount)
    {
        Items[item].SetAmount(amount);
        OnChange?.Invoke();
    }

    public bool HasItem(ItemType item)
    {
        return (ItemAmount(item) > 0);
    }

    public int ItemAmount(ItemType item)
    {
        return Items[item].amount;
    }
}
