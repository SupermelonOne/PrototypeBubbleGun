using UnityEngine.Events;
using System;
using System.Collections.Generic;
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