using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject arms;
    
    [HideInInspector] public PlayerInventory inventory;
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public PlayerInstrument instrument;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        controller = GetComponent<PlayerController>();
        instrument = arms.GetComponent<PlayerInstrument>();
        PlayerEventBus.Invoke(new PlayerEventBus.PlayerJoin(controller.playerCamera, this));

    }
}
