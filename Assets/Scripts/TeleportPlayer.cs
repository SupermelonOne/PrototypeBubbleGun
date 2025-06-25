using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private Transform targetLocation;
    [HideInInspector] public Transform player;

    public void ActivateTeleportation()
    {
        if (player == null || targetLocation == null)
        {
            Debug.LogWarning("Teleport failed: player or targetLocation is not set.");
            return;
        }

        Vector3 distance = targetLocation.position - transform.position;

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false; // disable to allow position change
            player.position += distance;
            controller.enabled = true;
        }
        else
        {
            player.position += distance;
        }

        Debug.Log("Teleported player by distance: " + distance);
    }
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
    public void ResetPlayer()
    {
        player = null;
    }

}
