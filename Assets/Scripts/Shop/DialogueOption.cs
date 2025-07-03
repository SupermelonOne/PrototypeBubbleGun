using UnityEngine.Events;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[Serializable]
public class Option
{
    public string name;
    [TextArea]
    public string description;

    [HideInInspector] public int id;
    [HideInInspector] public int layer;
}

[Serializable]
public class DialogueOption : Option
{
    [SerializeReference] public List<Option> options = new();
}


[Serializable]
public class BuyOption : Option
{
    public ItemType item;
    public int amount;
    public int price;

    [HideInInspector]
    public DialogueManager managerReference; // manually assigned or injected


    public void InvokeSelection(DialogueManager manager)
    {
        if (managerReference == null)
            managerReference = manager;
        if (managerReference != null)
        {
            managerReference.HandleBuyOption(this);
        }
    }}



//The names aren't very imaginative, everything is called Serializable. That's confusing for the player -Elin