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
    [SerializeField] private GameObject gloveObj;
    [SerializeField] private GameObject gunObj;
    private int activeInstrument;

    [SerializeField] private ShootBubble shootBubble;
    [SerializeField]private ScrubSponge scrubSponge;
    [SerializeField]private GrabBone grabBone;
    [SerializeField]private SprayWater sprayWater;
    private void Start()
    {
        scrubSponge = GetComponent<ScrubSponge>();
        grabBone = GetComponent<GrabBone>();
        sprayWater = GetComponent<SprayWater>();
        shootBubble = GetComponent<ShootBubble>();


        if (spongeObj == null)
            Debug.LogError("missing spongeObj");
        if (gloveObj == null)
            Debug.LogError("missing gloveObj");
        if (gunObj == null)
            Debug.LogError("missing gunObj");
        
        //TODO: fix this
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        SwitchWeapon(players.Length);
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
            case 4:
                grabBone.enabled = true;
                gloveObj.SetActive(true);
                break;
            default:
                Debug.LogError("invalid index");
                break;
        }
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
        if (activeInstrument > 4)
        {
            activeInstrument = 1;
        }
        if (activeInstrument < 1)
        {
            activeInstrument = 4;
        }

        DisableAll();
        SelectWeapon(activeInstrument);
    }

    private void DisableAll()
    {
        //no null checks because they cant be null
        gunObj.SetActive(false);
        shootBubble.enabled = false;


        sprayWater.enabled = false;

        spongeObj.SetActive(false);
        scrubSponge.enabled = false;

        gloveObj.SetActive(false);
        grabBone.enabled = false;

    }
}
