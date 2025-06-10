using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TwoPlayerPresent : MonoBehaviour
{
    private PlayerController player;
    // TODO add functionality to the playerscripts so they can move around and do the stuff with the monster when in/around the cage/veranda
    private List<PlayerController> players = new List<PlayerController>();
    [SerializeField] private TwoPlayerPresent otherActivator;
    [HideInInspector] public bool ready = false;
    [HideInInspector] public bool bothReady = false;
    [SerializeField] private UnityEvent activateFunction;
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
                Activate();
                otherActivator.Activate();
                activateFunction.Invoke();
            }
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
}
