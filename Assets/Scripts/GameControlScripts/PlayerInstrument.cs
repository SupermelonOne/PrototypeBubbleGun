using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerInstrument : MonoBehaviour
{
    private bool swapped = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            swapped = !swapped;
            enabledSlots.Clear();
            if (swapped)
            {
                enabledSlots.Add(0);
                enabledSlots.Add(1);
                enabledSlots.Add(2);
                enabledSlots.Add(3);
                enabledSlots.Add(4);
            }
            else
            {
                enabledSlots.Add(2);
                enabledSlots.Add(3);
            }
        }
    }
    private int activeInstrument;
    private int failSafe = 0;
    [SerializeField] private List<PlayerAction> instruments = new List<PlayerAction>();
    [SerializeField] private List<GameObject> instrumentObjects = new List<GameObject>();
    private List<int> enabledSlots = new List<int>();
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
            failSafe = 0;
            SwitchWeapon(1);
        }
    }
    public void SwitchRight(InputAction.CallbackContext button)
    {
        if (button.started)
        {
            failSafe = 0;
            SwitchWeapon(-1);
        }
    }

    private void SwitchWeapon(int direction)
    {
        failSafe++;
        if (failSafe > instruments.Count)
        {
            return;
        }
        activeInstrument += direction;
        if (activeInstrument >= instruments.Count)
        {
            activeInstrument = 0;
        }
        if (activeInstrument < 0)
        {
            activeInstrument = instruments.Count-1;
        }
        //Debug.Log(activeInstrument);
        if (!enabledSlots.Contains(activeInstrument))
        {
            SwitchWeapon(direction);
        }
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

/*    public void ChangeWeapon(List<PlayerAction> actions, List<GameObject> objects)
    {
        instruments.Clear();
        instrumentObjects.Clear();
        instruments.AddRange(actions);
        instrumentObjects.AddRange(objects);    
    }*/
    public void SwapEquipment(List<int> newEquipment)
    {
        enabledSlots.Clear();
        enabledSlots.AddRange(newEquipment);
        SwitchWeapon(1);
    }
}
