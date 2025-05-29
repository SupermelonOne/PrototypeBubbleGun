using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform respawnPosition;


    [SerializeField] private string Horizontal = "Horizontal";
    [SerializeField] private string Vertical = "Vertical";

    [SerializeField] private string camHorizontal = "Mouse X";
    [SerializeField] private string camVertical = "Mouse Y";
    [SerializeField] private float sensitivity = 2.0f;

    [SerializeField] private string jumpButton = "p1Jump";

    private bool jumpInput = false;

    private Vector2 m_moveAmt = Vector2.zero;
    private Vector2 m_lookAmt = Vector2.zero;

    private Vector3 verticalMovement = Vector3.zero;

    private CharacterController characterController;
    [SerializeField] private float speedModifier = 2;
    [SerializeField] Camera playerCamera;
    [SerializeField] private float gravity  = 20.0f;
    public float lookXLimit = 45.0f;
    float rotationX = 0;
    [SerializeField] private float jumpforce = 5;


    private PlayerInputActions mPlayerInput;
    public PlayerInput playerInput;

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

            foreach (var action in GetComponents<PlayerAction>())
            {
                mPlayerInput.GamePad.Shoot.performed += action.OnFire;
                mPlayerInput.GamePad.Shoot.started += action.OnFire;
                mPlayerInput.GamePad.Shoot.canceled += action.OnFire;
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

    private void Start()
    {   
        //TODO: make players spawn here when they first join, also lets not use GameObject.Find
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        
        GameObject respawnObj = GameObject.Find("SpawnPlace");
        respawnPosition = respawnObj.transform;
        
        PlayerEventBus.Invoke(new PlayerJoin(playerCamera));
    }

    void Update()
    {
        if (characterController == null) return;
        
        rotationX += -m_lookAmt.y * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, m_lookAmt.x * sensitivity, 0);

        if (jumpInput && characterController.isGrounded)
            verticalMovement.y = jumpforce;
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
