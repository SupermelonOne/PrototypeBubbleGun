using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IndependanChangeInstruments : MonoBehaviour
{
    [SerializeField] private List<int> instruments = new List<int>();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("found player");
            PlayerInstrument playerInstruments = other.GetComponentInChildren<PlayerInstrument>();
            //IF THIS GIVES NULL ERRORSS, WE PROBABLY MOVED THE PLAYERINSTRUMENT SCRIPT
            playerInstruments.SwapEquipment(instruments);
        }
    }
}
