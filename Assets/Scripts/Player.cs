using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject arms;
    
    [HideInInspector] public PlayerInventory inventory;
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public PlayerInstrument instrument;
    [HideInInspector] public PlayerGUI gui;

    private void Awake()
    {
        gui = GetComponentInChildren<PlayerGUI>(true);
        inventory = PlayerInventory.Instance;
        controller = GetComponent<PlayerController>();
        instrument = arms.GetComponent<PlayerInstrument>();
        PlayerEventBus.Invoke(new PlayerEventBus.PlayerJoin(controller.playerCamera, this));
        
        gui.AssignPlayer(this);
    }
}
