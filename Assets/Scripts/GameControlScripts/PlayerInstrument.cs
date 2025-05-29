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
    [SerializeField] private GameObject spongeObj;
    [SerializeField] private GameObject gunObj;
    private int activeInstrument;

    private ShootBubble shootBubble;
    private ScrubSponge scrubSponge;
    private SprayWater sprayWater;
    private void Start()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        SwitchWeapon(players.Length);
        
        scrubSponge = GetComponent<ScrubSponge>();
        sprayWater = GetComponent<SprayWater>();
        shootBubble = GetComponent<ShootBubble>();
        
        if (spongeObj == null)
            Debug.LogError("missing spongeObj");
        if (gunObj == null)
            Debug.LogError("missing gunObj");
    }

    private void SelectWeapon(int index)
    {
        switch (index)
        {
            case 1:
                shootBubble.enabled = true;
                gunObj.SetActive(true);
                break;
            case 2:
                sprayWater.enabled = true;
                gunObj.SetActive(true);
                break;
            case 3:
                scrubSponge.enabled = true;
                spongeObj.SetActive(true); 
                break;
            default:
                Debug.LogError("invalid index");
                break;
        }
    }

    public void SwitchLeft(InputAction.CallbackContext button)
    {
        SwitchWeapon(1);
    }
    public void SwitchRight(InputAction.CallbackContext button)
    {
        SwitchWeapon(-1);
    }

    private void SwitchWeapon(int direction)
    {
        activeInstrument += direction;
        if (activeInstrument > 3)
        {
            activeInstrument = 1;
        }
        if (activeInstrument < 1)
        {
            activeInstrument = 3;
        }

        DisableAll();
        SelectWeapon(activeInstrument);
    }

    private void DisableAll()
    {
        //no null checks because they cant be null
        spongeObj.SetActive(false);
        gunObj.SetActive(false);
        shootBubble.enabled = false;
        sprayWater.enabled = false;
        scrubSponge.enabled = false;
    }
}
