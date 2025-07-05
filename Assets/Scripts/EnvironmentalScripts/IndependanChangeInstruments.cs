using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IndependanChangeInstruments : MonoBehaviour
{
    [SerializeField] private List<int> instruments = new List<int>();
    [SerializeField] private bool GrabberAdder = false;
    [SerializeField] private bool SoapAdder = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwapInstruments(other.gameObject);
        }
    }

    private void SwapInstruments(GameObject other)
    {
        var inv = PlayerInventory.Instance;
        if (inv == null) return;
        if (GrabberAdder)
        {
            if (inv.HasItem(ItemType.Grabber)) instruments.Add(3);
        }
        if (SoapAdder)
        {
            if (inv.HasItem(ItemType.SoapGun)) instruments.Add(5);
        }

        //Debug.Log("found player");
        PlayerInstrument playerInstruments = other.GetComponentInChildren<PlayerInstrument>();
        //IF THIS GIVES NULL ERRORSS, WE PROBABLY MOVED THE PLAYERINSTRUMENT SCRIPT
        playerInstruments.SwapEquipment(instruments);
        
    }
}
