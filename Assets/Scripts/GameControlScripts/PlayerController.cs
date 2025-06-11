using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{

    //pretty sure these are useless but im keepin em here just in case
    /*[SerializeField] private string Horizontal = "Horizontal";
    [SerializeField] private string Vertical = "Vertical";
    [SerializeField] private string camHorizontal = "Mouse X";
    [SerializeField] private string camVertical = "Mouse Y";
    [SerializeField] private string jumpButton = "p1Jump";*/

    
    [HideInInspector] public PlayerInput playerInput;
    [SerializeField] public float lookXLimit = 45.0f;
    
    [SerializeField] private Transform respawnPosition;
    [SerializeField] public Camera playerCamera;
    
    [SerializeField] private float sensitivity = 2.0f;
    [SerializeField] private float speedModifier = 2;
    [SerializeField] private float gravity  = 20.0f;
    [SerializeField] private float jumpForce = 5;


    private Vector2 m_moveAmt = Vector2.zero;
    private Vector2 m_lookAmt = Vector2.zero;
    private Vector3 verticalMovement = Vector3.zero;
    private PlayerInputActions mPlayerInput;
    private CharacterController characterController;
    
    private bool jumpInput;
    private float rotationX;


    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        
        var respawnObj = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");
        respawnPosition = respawnObj.transform;
    }

    private void OnEnable()
    {
        mPlayerInput = new PlayerInputActions();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            playerInput.actions = mPlayerInput.asset;

            mPlayerInput.GamePad.Jump.performed += OnJump;
            mPlayerInput.GamePad.Jump.started += OnJump;
            mPlayerInput.GamePad.Jump.canceled += OnJump;

            foreach (var action in GetComponentsInChildren<PlayerAction>())
            {
                mPlayerInput.GamePad.Shoot.performed += action.OnFire;
                mPlayerInput.GamePad.Shoot.started += action.OnFire;
                mPlayerInput.GamePad.Shoot.canceled += action.OnFire;
            }
            
            PlayerInstrument playerInstrument = GetComponentInChildren<PlayerInstrument>();
            if (playerInstrument != null)
            {
                mPlayerInput.GamePad.SwapLeft.started += playerInstrument.SwitchLeft;
                mPlayerInput.GamePad.SwapRight.started += playerInstrument.SwitchRight;
            }
        }
    }
    private void OnDisable()
    {
        if (mPlayerInput != null)
        {
            mPlayerInput.GamePad.Jump.performed -= OnJump;
            mPlayerInput.GamePad.Jump.started -= OnJump;
            mPlayerInput.GamePad.Jump.canceled -= OnJump;
            
            foreach (var action in GetComponents<PlayerAction>())
            {
                mPlayerInput.GamePad.Shoot.performed -= action.OnFire;
                mPlayerInput.GamePad.Shoot.started -= action.OnFire;
                mPlayerInput.GamePad.Shoot.canceled -= action.OnFire;
            }
            
            PlayerInstrument playerInstrument = GetComponent<PlayerInstrument>();
            if (playerInstrument != null)
            {
                mPlayerInput.GamePad.SwapLeft.started -= playerInstrument.SwitchLeft;
                mPlayerInput.GamePad.SwapRight.started -= playerInstrument.SwitchRight;
            }
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        m_moveAmt = ctx.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        m_lookAmt = ctx.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            jumpInput = true;
        if (context.canceled)
            jumpInput = false;
    }

    void Update()
    {
        if (characterController == null) return;
        
        rotationX += -m_lookAmt.y * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, m_lookAmt.x * sensitivity, 0);

        if (jumpInput && characterController.isGrounded)
            verticalMovement.y = jumpForce;
        else if (!characterController.isGrounded)
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

        if (respawnPosition != null && transform.position.y < -90)
        {
            transform.position = respawnPosition.position;
        }
    }
}
