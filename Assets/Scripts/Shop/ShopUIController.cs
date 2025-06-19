using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ShopUIController : MonoBehaviour
{
    [HideInInspector] public PlayerInput playerInput;


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
            characterController = GetComponent<CharacterController>(); }

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
}
