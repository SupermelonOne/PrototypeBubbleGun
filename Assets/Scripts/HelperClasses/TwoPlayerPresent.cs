using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TwoPlayerPresent : MonoBehaviour
{
    //[SerializeField] private bool twoPlayersRequired = true;

    private PlayerController player;
    // TODO add functionality to the playerscripts so they can move around and do the stuff with the monster when in/around the cage/veranda
    private List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private TwoPlayerPresent otherActivator;
    [HideInInspector] public bool ready = false;
    [HideInInspector] public bool bothReady = false;
    [SerializeField] private UnityEvent activateFunction;

    TeleportPlayer teleportPlayer;
    private void Start()
    {
        if (teleportPlayer == null)
            teleportPlayer = GetComponent<TeleportPlayer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null) return;
        players.Add(playerController);
        if (player == null)
        {
            ready = true;
            player = playerController;
            if (otherActivator == null) return;
            if (otherActivator.ready)
            {
                Debug.Log("second player entered, activated functions on: " + gameObject.name);
                Activate();
                otherActivator.Activate();
                activateFunction.Invoke();
                otherActivator.activateFunction.Invoke();
            }
            if (teleportPlayer == null)
                return;
            teleportPlayer.SetPlayer(playerController.transform);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController == null) return;
        if (player != null)
        {
            players.Remove(playerController);
            if (players.Count > 0)
            {
                player = players.First();
            }
            else
            {
                ready = false;
                player = null;
            }
            if (teleportPlayer == null)
                return;
            teleportPlayer.SetPlayer(player.transform);
        }
    }

    public void UnActivate()
    {
        bothReady = false;
    }
    public void Activate()
    {
        bothReady = true;
    }

    [SerializeField] private List<int> actions = new List<int>(); //maybe there is a better way but I feel like this one would run most efficiently
    private void ChangeWeapons(PlayerController target, List<int> _actions)
    {
        Debug.Log("done the thing");
        PlayerInstrument instruments = target.GetComponentInChildren<PlayerInstrument>();
        //IF THIS GIVES NULL ERRORSS, WE PROBABLY MOVED THE PLAYERINSTRUMENT SCRIPT
        instruments.SwapEquipment(_actions);
    }
    public void ActivateChange()
    {
        ChangeWeapons(player, actions);
    }
}