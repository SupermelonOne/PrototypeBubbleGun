using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndependantTeleportScript : MonoBehaviour
{
    [SerializeField] private Transform targetLocation;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("found player");
            Transform player = other.transform;
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
    }
}
