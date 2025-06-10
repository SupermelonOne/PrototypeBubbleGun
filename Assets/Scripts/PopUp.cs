using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Events;

[System.Serializable]
public class PlayerEvent : UnityEvent<Player> { }

public class PopUp : MonoBehaviour
{
    [SerializeField] private float minDistance;
    [SerializeField] private KeyCode key;
    public PlayerEvent onButtonPressed;
    
    private List<Player> players = new List<Player>();
    private Dictionary<Player, Camera> playerCams = new Dictionary<Player, Camera>();
    private Canvas canvas;
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        textMesh.alpha = 0;
        textMesh.text = key.ToString();
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
        players.Add(playerEvent.player);
        playerCams.Add(playerEvent.player, playerEvent.camera);
    }

    //i don't like update but this is just vector math so it should be fineeee
    private void Update()
    {
        Player nearestPlayer = null;
        var nearestDistance = Mathf.Infinity;
        foreach (var player in players)
        {
            var dist = Vector3.Distance(transform.position, player.transform.position);
            if (dist < minDistance && dist < nearestDistance)
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


    private void OnPlayerClose(Player closestPlayer)
    {
        canvas.worldCamera = playerCams[closestPlayer];
        canvas.transform.LookAt(closestPlayer.transform.position);
        canvas.transform.Rotate(Vector3.up, 180f);
        textMesh.alpha = 1;
        if(Input.GetKeyDown(key))
            onButtonPressed?.Invoke(closestPlayer);
    }
}
