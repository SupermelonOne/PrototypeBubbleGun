using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenBridge : MonoBehaviour
{
    [SerializeField] private float distance;
    [SerializeField] private BridgeScript bridgeScript;

    private List<Transform> players = new List<Transform>();
    private Dictionary<Transform, Camera> playerCams = new Dictionary<Transform, Camera>();
    private Canvas canvas;
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        textMesh.alpha = 0;
    }

    private void OnEnable()
    {
        PlayerEventBus.Subscribe<PlayerEventBus.PlayerJoin>(OnPlayerJoined);
    }

    private void OnDisable()
    {
        PlayerEventBus.UnSubscribe<PlayerEventBus.PlayerJoin>(OnPlayerJoined);
    }

    private void OnPlayerJoined(PlayerEventBus.PlayerJoin playerEvent)
    {
        players.Add(playerEvent.transform);
        playerCams.Add(playerEvent.transform, playerEvent.camera);
        
        Debug.Log("Player joined");
    }

    //i don't like update but this is just vector math so it should be fineeee
    private void Update()
    {
        Transform nearestPlayer = null;
        var nearestDistance = Mathf.Infinity;
        foreach (var player in players)
        {
            var dist = Vector3.Distance(transform.position, player.position);
            if (dist < distance && dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestPlayer = player;
            }
        }
        
        if (nearestPlayer != null)
            OnPlayerClose(nearestPlayer);
        else
            textMesh.alpha = 0;
    }


    private void OnPlayerClose(Transform closestPlayer)
    {
        Debug.Log($"eyo the player is close and this is getting called {closestPlayer}");
        canvas.worldCamera = playerCams[closestPlayer];
        canvas.transform.LookAt(closestPlayer.position);
        canvas.transform.Rotate(Vector3.up, 180f);
        textMesh.alpha = 1;
        if(Input.GetKeyDown(KeyCode.Y))
            bridgeScript.OnBuildBridgeWithDelay();
    }
}
