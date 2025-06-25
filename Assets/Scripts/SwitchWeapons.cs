using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWeapons : MonoBehaviour
{
    private PlayerInstrument activePlayer;
    [SerializeField] private List<int> actions = new List<int>(); //maybe there is a better way but I feel like this one would run most efficiently
    private void ChangeWeapons(PlayerInstrument target, List<int> _actions)
    {
        target.SwapEquipment(_actions);
    }
    public void ActivateChange()
    {
        ChangeWeapons(activePlayer, actions);
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerInstrument target = other.GetComponent<PlayerInstrument>();
        if (target != null)
            activePlayer = target;
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerInstrument target = other.GetComponent<PlayerInstrument>();
        if (target != null)
            activePlayer = null;
    }
}
