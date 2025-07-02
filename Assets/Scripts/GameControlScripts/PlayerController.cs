using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [SerializeField] ArduinoInputManager arduinoInputManager;

    //pretty sure these are useless but im keepin em here just in case
    /*[SerializeField] private string Horizontal = "Horizontal";
    [SerializeField] private string Vertical = "Vertical";
    [SerializeField] private string camHorizontal = "Mouse X";
    [SerializeField] private string camVertical = "Mouse Y";
    [SerializeField] private string jumpButton = "p1Jump";*/

    // I love the use of colour, keep it up -Elin
    [HideInInspector] public PlayerInput playerInput;
    [SerializeField] public float lookXLimit = 45.0f;
    
    [SerializeField] private Vector3 respawnPosition;
    [SerializeField] public Camera playerCamera;
    
    [SerializeField] private float sensitivity = 2.0f;
    [SerializeField] private float speedModifier = 2;
    [SerializeField] private float gravity  = 20.0f;
    [SerializeField] private float jumpForce = 5;

    [SerializeField] private float respawnSeconds = 1;

    private bool shopOpen = false;
    private bool inventoryOpen = false;
    
    private bool interactPossible = false;
    private Vector2 m_moveAmt = Vector2.zero;
    private Vector2 m_lookAmt = Vector2.zero;
    private Vector3 verticalMovement = Vector3.zero;
    private PlayerInputActions mPlayerInput;
    private CharacterController characterController;
    
    private bool jumpInput;
    private float rotationX;

    private Transform respawnTransform;
    private bool respawning = false;
    [SerializeField] LayerMask disabledOnRespawn;
    LayerMask enabledAfterRespawn = new LayerMask();
    private BlinkScript blinkScript;

    private bool justRespawned = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        
        var respawnObj = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");
        blinkScript = GetComponentInChildren<BlinkScript>();
    }

    private void OnEnable()
    {

        GameObject newRespawnObj = new GameObject("playerRespawnObj");
        respawnTransform = newRespawnObj.transform;

        mPlayerInput = new PlayerInputActions();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null) return;
        
        playerInput.actions = mPlayerInput.asset;

        mPlayerInput.GamePad.Jump.performed += OnJump;
        mPlayerInput.GamePad.Jump.started += OnJump;
        mPlayerInput.GamePad.Jump.canceled += OnJump;

        mPlayerInput.GamePad.Shoot.started += OnFire;
        mPlayerInput.GamePad.Shoot.performed += OnFire;
        mPlayerInput.GamePad.Shoot.canceled += OnFire;
  
            
        PlayerInstrument playerInstrument = GetComponentInChildren<PlayerInstrument>();
        if (playerInstrument != null)
        {
            mPlayerInput.GamePad.SwapLeft.started += playerInstrument.SwitchLeft;
            mPlayerInput.GamePad.SwapRight.started += playerInstrument.SwitchRight;
        }

        mPlayerInput.UIMap.Back.started += OnUIMoveBack;
        mPlayerInput.UIMap.Select.started += OnUIMoveSelect;
        mPlayerInput.UIMap.NavigateUp.started += OnUIMoveUp;
        mPlayerInput.UIMap.NavigateDown.started += OnUIMoveDown;
        mPlayerInput.UIMap.NavigateLeft.started += OnUIMoveLeft;
        mPlayerInput.UIMap.NavigateRight.started += OnUIMoveRight;
    }

    
    private void OnDisable()
    {
        if (mPlayerInput != null)
        {
            mPlayerInput.GamePad.Jump.performed -= OnJump;
            mPlayerInput.GamePad.Jump.started -= OnJump;
            mPlayerInput.GamePad.Jump.canceled -= OnJump;
            
            mPlayerInput.GamePad.Shoot.started -= OnFire;
            mPlayerInput.GamePad.Shoot.performed -= OnFire;
            mPlayerInput.GamePad.Shoot.canceled -= OnFire;

            
            PlayerInstrument playerInstrument = GetComponent<PlayerInstrument>();
            if (playerInstrument != null)
            {
                mPlayerInput.GamePad.SwapLeft.started -= playerInstrument.SwitchLeft;
                mPlayerInput.GamePad.SwapRight.started -= playerInstrument.SwitchRight;
            }
            
            mPlayerInput.UIMap.Back.started -= OnUIMoveBack;
            mPlayerInput.UIMap.Select.started -= OnUIMoveSelect;
            mPlayerInput.UIMap.NavigateUp.started -= OnUIMoveUp;
            mPlayerInput.UIMap.NavigateDown.started -= OnUIMoveDown;
            mPlayerInput.UIMap.NavigateLeft.started -= OnUIMoveLeft;
            mPlayerInput.UIMap.NavigateRight.started -= OnUIMoveRight;  
        }
    }
    

    public void ToggleShopUI(bool isOpen)
    {
        shopOpen = isOpen;
        OnUI(isOpen);
    }

    public void SetInventory(bool isOpen)
    {
        inventoryOpen = isOpen;
        OnUI(isOpen);
    }

    private void OnUI(bool isOpen)
    {
        if (isOpen)
        {
            mPlayerInput.GamePad.Disable(); // Disable gameplay input
            mPlayerInput.UIMap.Enable();  // Enable shop UI input
        }
        else
        {
            mPlayerInput.UIMap.Disable();
            mPlayerInput.GamePad.Enable();
        }
    }
    
    

    private void OnUIMoveDown(InputAction.CallbackContext context)
    {
        OnUIMove(InputTypes.Down);
    }
    private void OnUIMoveUp(InputAction.CallbackContext context)
    {
        OnUIMove(InputTypes.Up);    
    }

    private void OnUIMoveLeft(InputAction.CallbackContext context)
    {
        OnUIMove(InputTypes.Left);
    }

    private void OnUIMoveRight(InputAction.CallbackContext context)
    {
        OnUIMove(InputTypes.Right);
    }
    private void OnUIMoveSelect(InputAction.CallbackContext context)
    {
        OnUIMove(InputTypes.Select);
    }
    private void OnUIMoveBack(InputAction.CallbackContext context)
    {
        OnUIMove(InputTypes.Back);
    }

    private void OnUIMove(InputTypes type)
    {
        if(shopOpen)
            ShopEventBus.Invoke(new ShopEventBus.OnNavigateUI(type));
        else 
            InventoryEventBus.Invoke(new InventoryEventBus.OnNavigateUI(type));
            
    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (interactPossible)
        {
            return;
        }

        var c = GetComponentsInChildren<PlayerAction>();
        foreach (var action in c)
        {
            action.OnFire(context);
        }
    }

    public void OnFireAction()
    {

    }

    public void InteractionToggle(bool isOpen)
    {
        interactPossible = isOpen;
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (arduinoInputManager != null)
            return;
        OnMoveAction(ctx.ReadValue<Vector2>());
    }
    
    private Vector2 FixArduinoVectorHorizontal(Vector2 vec2)
    {
        if (vec2.x > 0)
        {
            vec2.x *= 2;
            if (vec2.x > 1)
            {
                vec2.x = 1;
            }
        }
        if (vec2.y > 0)
        {
            vec2.y *= 2;
            if (vec2.y > 1)
            {
                vec2.y = 1;
            }
        }

        vec2.x *= -1;
        vec2.y *= -1;
        if (vec2.x > -0.35 && vec2.x < 0.35)
        {
            vec2.x = 0;
        }
        if (vec2.y > -0.35 && vec2.y < 0.35)
        {
            vec2.y = 0;
        }
        if (vec2.x != 0)
        {
            if (vec2.x > 0)
            {
                vec2.x -= 0.35f;
                vec2.x *= 1.2f;
            }
            if (vec2.x < 0)
            {
                vec2.x += 0.35f;
                vec2.x *= 1.538f;
            }
        }
        if (vec2.y != 0)
        {
            if (vec2.y > 0)
            {
                vec2.y -= 0.35f;
                vec2.y *= 1.2f;
            }
            if (vec2.y < 0)
            {
                vec2.y += 0.35f;
                vec2.y *= 1.538f;
            }
        }
        return vec2;
    }

    public void OnMoveAction(Vector2 vec2)
    {
        if (arduinoInputManager != null)
        {
            vec2 = FixArduinoVectorHorizontal(vec2);
        }
        m_moveAmt = vec2;
        justRespawned = false;
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {

        if (arduinoInputManager != null)
            return;
        OnLookAction(ctx.ReadValue<Vector2>());
    }
    public void OnLookAction(Vector2 vec2)
    {
        if (arduinoInputManager != null)
        {
            vec2 = FixArduinoVectorHorizontal(-vec2);
            vec2.x *= -1f;
            vec2.y *= -1f;
            Debug.Log(vec2);
        }
        m_lookAmt = vec2;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (arduinoInputManager != null)
            return;
        //TODO: vezko istg what is this
        if (context.started)
        {
            jumpInput = true;
            justRespawned = false;
        }
        if (context.canceled)
            jumpInput = false;
    }
    
    // <summary>
    /// Returns <c>true</c> for as long as the Shoot action is held down.
    /// </summary>
    public bool IsFirePressed()
    {
        return mPlayerInput != null && mPlayerInput.GamePad.Shoot.IsPressed();
    }

    void Update()
    {
        if (characterController == null) return;

        if (arduinoInputManager != null)
        {
            OnMoveAction(arduinoInputManager.Joystick1);
            OnLookAction(arduinoInputManager.Joystick2);
            jumpInput = arduinoInputManager._button1;
        }

        if (!respawning)
        {
            if (characterController.isGrounded)
            {
                respawnTransform.position = transform.position;
            }

            rotationX += -m_lookAmt.y * sensitivity;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, m_lookAmt.x * sensitivity, 0);

            if (jumpInput && characterController.isGrounded || jumpInput && justRespawned)
                verticalMovement.y = jumpForce;
            else if (!characterController.isGrounded && !justRespawned)
                verticalMovement.y -= gravity * Time.deltaTime;
            else if (!jumpInput)
                verticalMovement.y = 0;


            var direction = new Vector3(m_moveAmt.x, 0, m_moveAmt.y);

            if (direction.magnitude > 1)
                direction.Normalize();

            var camForward = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z).normalized;
            var camRight = new Vector3(playerCamera.transform.right.x, 0, playerCamera.transform.right.z).normalized;

            var desiredDirection = (direction.z * camForward + direction.x * camRight) * speedModifier;


            desiredDirection += verticalMovement;
            characterController.Move(desiredDirection * Time.deltaTime);

        }
        else
        {
            Vector3 direction = (respawnTransform.position - transform.position).normalized;
            float respawnSpeed = Vector3.Distance(respawnTransform.position, transform.position);
            characterController.Move((direction * respawnSpeed * Time.deltaTime) / respawnSeconds + 8 * direction * Time.deltaTime);
            if (Vector3.Distance(respawnTransform.position, transform.position) < 0.1f)
            {
                respawning = false;
                characterController.excludeLayers = enabledAfterRespawn;
                justRespawned = true;
                if (blinkScript == null)
                    return;
                blinkScript.isBlinking = false;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RespawnZone"))
        {
            characterController.excludeLayers = disabledOnRespawn;
            respawning = true;
            if (blinkScript == null)
                return;
            verticalMovement.y = 0;
            blinkScript.isBlinking = true;
        }
    }

    // BACKUP FOR REAL CAM CONTROLS
    /*    private Vector2 lookInput = Vector2.zero;

    [SerializeField] private Transform camPivX;
    [SerializeField] private Transform camPivY;

    public void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }
    */
}
