using UnityEngine.Events;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[Serializable]
public class DialogueOption 
{
    public string dialogueName;
    [TextArea]
    public string dialogue;

    [HideInInspector] public List<DialogueOption> options = new();
    public UnityEvent onSelected;


    [HideInInspector] public int ID;
    [HideInInspector] public int layer;
}


[Serializable]
public class BuyOption 
{
    public string ItemName;
    [TextArea]
    public string discription;
    public ItemType item;
    public int amount;
    public int price;
    


    public UnityEvent onSelected;

    [HideInInspector] public int ID;
    [HideInInspector] public int layer;
}

//The names aren't very imaginative, everything is called Serializable. That's confusing for the player -Elin