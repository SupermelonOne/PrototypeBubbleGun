using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerInstrument : MonoBehaviour
{
    int activeinstrument = 0;
    [SerializeField] private GameObject spongeObj;
    [SerializeField] private GameObject gunObj;
    private void Start()
    {
        SecondStolenController[] players = FindObjectsOfType<SecondStolenController>();
        //activeinstrument = players.Length;
        SwitchWeapon(players.Length);
        
    }

    private void SelectWeapon()
    {

        switch (activeinstrument)
        {
            case 1:
                ShootBubble shootBubble = GetComponent<ShootBubble>();
                if (shootBubble != null)
                    shootBubble.enabled = true;
                if (gunObj != null)
                    gunObj.SetActive(true);
                else
                    Debug.Log("missing ShootBubble script");
                break;
            case 2:
                SprayWater sprayWater = GetComponent<SprayWater>();
                if (sprayWater != null)
                    sprayWater.enabled = true;
                if (gunObj != null)
                    gunObj.SetActive(true);
                else
                    Debug.Log("missing SprayWater script");
                break;
            case 3:
                ScrubSponge scrubSponge = GetComponent<ScrubSponge>();
                if (scrubSponge != null)
                    scrubSponge.enabled = true;
                if (spongeObj != null)
                    spongeObj.SetActive(true);
                else
                    Debug.Log("missing SpongeScript script");
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

    public void SwitchWeapon(int direction)
    {
        activeinstrument += direction;
        if (activeinstrument > 3)
        {
            activeinstrument = 1;
        }
        if (activeinstrument < 1)
        {
            activeinstrument = 3;
        }

        DisableAll();
        SelectWeapon();
    }

    private void DisableAll()
    {
        if (spongeObj != null)
            spongeObj.SetActive(false);
        if (gunObj != null)
            gunObj.SetActive(false);
        ShootBubble shootBubble = GetComponent<ShootBubble>();
        if (shootBubble != null)
            shootBubble.enabled = false;
        else
            Debug.Log("missing ShootBubble script");

        SprayWater sprayWater = GetComponent<SprayWater>();
        if (sprayWater != null)
            sprayWater.enabled = false;
        else
            Debug.Log("missing SprayWater script");

        ScrubSponge scrubSponge = GetComponent<ScrubSponge>();
        if (scrubSponge != null)
            scrubSponge.enabled = false;
        else
            Debug.Log("missing SpongeScript script");
    }
}
