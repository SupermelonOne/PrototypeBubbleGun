using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] private Transform targetLocation;
    [HideInInspector] public Transform player;

/*    public void MovePlayer(Transform player)
    {
        Vector3 distance = transform.position - targetLocation.position;
        player.transform.position += distance;
    }*/

    public void ActivateTeleportation()
    {
        Vector3 distance = transform.position - targetLocation.position;
        player.transform.position += distance;
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
