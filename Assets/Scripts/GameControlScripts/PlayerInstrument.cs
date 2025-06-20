using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;
[RequireComponent(typeof(ShootBubble))]
[RequireComponent(typeof(SprayWater))]
[RequireComponent(typeof(ScrubSponge))]

public class PlayerInstrument : MonoBehaviour
{
    private int activeInstrument;

    [SerializeField] private List<PlayerAction> instruments = new List<PlayerAction>();
    [SerializeField] private List<GameObject> instrumentObjects = new List<GameObject>();
    private void Start()
    {
        if (instruments.Count <= 0)
        {
            instruments = GetComponentsInChildren<PlayerAction>().ToList();
        }


        PlayerController[] players = FindObjectsOfType<PlayerController>();
        SwitchWeapon(players.Length);
    }

    private void SelectWeapon(int index)
    {
        if (instruments.Count > index)
            instruments[index].enabled = true;
        if (instrumentObjects.Count > index)
            instrumentObjects[index].SetActive(true);
        //maybe set it up in such a way where every playerAction script has an attached gameObject, which u enable or disable here, either thru its own code and calling the function or doing it in here
    }
    
   

    public void SwitchLeft(InputAction.CallbackContext button)
    {
        if (button.started)
        {
            SwitchWeapon(1);
        }
    }
    public void SwitchRight(InputAction.CallbackContext button)
    {
        if (button.started)
        {
            SwitchWeapon(-1);
        }
    }

    private void SwitchWeapon(int direction)
    {
        activeInstrument += direction;
        if (activeInstrument >= instruments.Count)
        {
            activeInstrument = 0;
        }
        if (activeInstrument < 0)
        {
            activeInstrument = instruments.Count-1;
        }
        Debug.Log(activeInstrument);

        DisableAll();
        SelectWeapon(activeInstrument);
    }

    private void DisableAll()
    {
        foreach(PlayerAction instrument in instruments)
        {
            instrument.enabled = false;
        }
        foreach(GameObject instrumentObject in instrumentObjects)
        {
            instrumentObject.SetActive(false);
        }
    }
}
