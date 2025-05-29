using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SecondStolenController : MonoBehaviour
{
    // OMG THIS CODE :(

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


    private PlayerInputActions m_playerInput;
    public PlayerInput playerInput;

    private void OnEnable()
    {
        m_playerInput = new PlayerInputActions();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            playerInput.actions = m_playerInput.asset;

            m_playerInput.GamePad.Jump.performed += OnJump;
            m_playerInput.GamePad.Jump.started += OnJump;
            m_playerInput.GamePad.Jump.canceled += OnJump;

            foreach (var action in GetComponents<PlayerAction>())
            {
                m_playerInput.GamePad.Shoot.performed += action.OnFire;
                m_playerInput.GamePad.Shoot.started += action.OnFire;
                m_playerInput.GamePad.Shoot.canceled += action.OnFire;
            }
        }
    }
    private void OnDisable()
    {
        if (m_playerInput != null)
        {
            m_playerInput.GamePad.Jump.performed -= OnJump;
            m_playerInput.GamePad.Jump.started -= OnJump;
            m_playerInput.GamePad.Jump.canceled -= OnJump;
            
            foreach (var action in GetComponents<PlayerAction>())
            {
                m_playerInput.GamePad.Shoot.performed -= action.OnFire;
                m_playerInput.GamePad.Shoot.started -= action.OnFire;
                m_playerInput.GamePad.Shoot.canceled -= action.OnFire;
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
        {   
            jumpInput = true;
        }
        if (context.canceled)
        {
            jumpInput = false;
        }
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

        rotationX += -m_lookAmt.y * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, m_lookAmt.x * sensitivity, 0);

        if (!characterController.isGrounded)
        {
            verticalMovement.y -= gravity * Time.deltaTime;
        }
        else if (!jumpInput)
        {
            verticalMovement.y = 0;
        }
        if (jumpInput && characterController.isGrounded)
        {
            if (characterController != null && characterController.isGrounded)
            {
                verticalMovement.y = jumpforce;
            }
        }

        Vector3 direction = new Vector3(m_moveAmt.x, 0, m_moveAmt.y);
        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }
        Vector3 camForward = playerCamera.transform.forward;
        camForward.y = 0;
        camForward.Normalize();
        Vector3 camRight = playerCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();
        Vector3 desiredDirection = (direction.z * camForward + direction.x * camRight);
        desiredDirection *= speedModifier;



        desiredDirection += verticalMovement;
        characterController.Move(desiredDirection * Time.deltaTime);

        if (respawnPosition != null && transform.position.y < -90)
        {
            transform.position = respawnPosition.position;
        }
    }
}
