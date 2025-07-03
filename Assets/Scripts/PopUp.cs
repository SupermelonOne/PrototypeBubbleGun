using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

#region SerializableShiz

[System.Serializable]
public class PlayerEvent : UnityEvent<Player> { }

[System.Serializable]

public struct Cost
{
    public ItemType itemType;
    public int cost;
}

[System.Serializable]
public class TextProperties
{
    [Tooltip("The text dimensions in unity units.")]
    public Vector2 textDimensions;
    [Tooltip("The text color.")]
    public Color textColor;
    [Tooltip("The height the text hovers over the box.")]
    public float textHoverHeight;
}

[System.Serializable]
public class InteractionConditions
{
    [Tooltip("Minimum distance to the object for interaction.")]
    public float minDistance = 0;
    [Tooltip("the width of the viewcast in degrees.")]
    public float viewWidth = 5f;
    [Tooltip("Key to press to trigger interaction.")]
    public KeyCode key = KeyCode.E;
}

[System.Serializable]
public class InteractionEffects
{
    [Tooltip("Event invoked when the interaction button is pressed.")]
    public PlayerEvent onButtonPressed;
    [Tooltip("If true, this object will be destroyed after successful interaction.")]
    public bool isDestroyedAfterPressed = false;
}

[System.Serializable]
public class RequirementsAndFeedback
{
    [Tooltip("Cost associated with this interaction (e.g., currency, item).")]
    public Cost cost;
    [Tooltip("Time in seconds to display an error message if interaction fails.")]
    public float errorMessageTime = 1.0f;
}

#endregion


public class PopUp : MonoBehaviour
{
    [SerializeField] private TextProperties textProperties;
    [SerializeField] private InteractionConditions conditions;
    [SerializeField] private InteractionEffects effects;
    [SerializeField] private RequirementsAndFeedback feedback;
    
    
    private float minDistance;
    private KeyCode key;

    private PlayerEvent onButtonPressed; // Consider making this private if it's only invoked internally
    private bool isDestroyedAfterPressed;
    
    private Cost cost;
    private float errorMessageTime;
    
    
    private List<Player> players = new List<Player>();
    private Dictionary<Player, Camera> playerCams = new Dictionary<Player, Camera>();
    private Canvas canvas;
    private TextMeshProUGUI tMesh;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        tMesh = GetComponentInChildren<TextMeshProUGUI>();
        
        minDistance = conditions.minDistance;
        key = conditions.key;
        
        onButtonPressed = effects.onButtonPressed;
        isDestroyedAfterPressed = effects.isDestroyedAfterPressed;
        
        cost = feedback.cost;
        errorMessageTime = feedback.errorMessageTime;
        
                
        tMesh.alpha = 0;
        tMesh.text = key.ToString();
        tMesh.color = textProperties.textColor;
        tMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f); // Center horizontal and vertical anchors
        tMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f); // Center horizontal and vertical anchors
        tMesh.rectTransform.pivot = new Vector2(0.5f, 0.5f);     // Center pivot
        tMesh.rectTransform.anchoredPosition = new Vector2(0f, textProperties.textHoverHeight);
        tMesh.rectTransform.sizeDelta = new Vector2(textProperties.textDimensions.x, tMesh.rectTransform.sizeDelta.y);   
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
            if (player == null) return;
            
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
            SetActive(true);
    }


    private void OnPlayerClose(Player closestPlayer)
    {
        var coneWidth = conditions.viewWidth;
        var halfWidth = coneWidth / 2f;
        var coneLength = Mathf.Sqrt((halfWidth * halfWidth) + minDistance * minDistance);
        var sinAngle = halfWidth / coneLength;
        var coneAngleRadians = Mathf.Asin(sinAngle);
        var coneAngleDegrees = coneAngleRadians * Mathf.Rad2Deg;

        
        
        
        var rayCount = 10;
        
        var isHitting  = false;
        
        for (int i = 0; i < rayCount; i++)
        {
            float angleStep = (coneAngleDegrees * 2f) / (rayCount - 1);
            float angleH = -coneAngleDegrees + (i * angleStep);

            var forward = closestPlayer.GetComponentInChildren<Camera>().transform.forward;
            var direction = Quaternion.Euler(0f, angleH, 0f) * forward;
            
            var playerPos = closestPlayer.controller.playerCamera.transform.position;
            if (Physics.Raycast(playerPos, direction, out RaycastHit hit, minDistance))
            {
                if (hit.collider == tMesh.gameObject.GetComponent<BoxCollider>())
                {
                    OnPlayerLook(closestPlayer);
                    isHitting = true;
                    break;
                }
            }

        }
        if (!isHitting)
            SetActive(false, closestPlayer);

    }

    private void OnPlayerLook(Player closestPlayer)
    {
        canvas.worldCamera = playerCams[closestPlayer];
        canvas.transform.LookAt(closestPlayer.transform.position);
        canvas.transform.Rotate(Vector3.up, 180f);
        SetActive(true, closestPlayer);
        closestPlayer.controller.InteractionToggle(true);
        
        if(closestPlayer.controller.IsFirePressed())
            IsPressed(closestPlayer);
    }

    private void IsPressed(Player closestPlayer)
    {
        var amount = closestPlayer.inventory.ItemAmount(cost.itemType);
        if (amount > cost.cost || cost.cost <= 0)
        {
            onButtonPressed?.Invoke(closestPlayer);
            
            if (isDestroyedAfterPressed)
                Destroy(gameObject);
        }
        else
        {
            StartCoroutine(ErrorMessage(errorMessageTime));
        }
    }

    private void SetActive(bool active, Player closestPlayer = null)
    {
        tMesh.alpha = active ? 1 : 0;
        if (closestPlayer != null)
            closestPlayer.gui.OnInteract(active);
    }

    private IEnumerator ErrorMessage(float seconds)
    {
        var c = tMesh.color;
        var t = tMesh.text;
        tMesh.color = Color.red;
        tMesh.text = $"you're broke, you need {cost.itemType}, and  {cost.cost} of em";
        yield return new WaitForSeconds(seconds);
        tMesh.color = c;
        tMesh.text = t;
    }
}

