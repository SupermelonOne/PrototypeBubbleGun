using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SecondStolenController : MonoBehaviour
{
    //private PlayerInput playerInput;

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

/*    private void Awake()
    {
        playerInput = new PlayerInput();
    }*/
    private void OnEnable()
    {
        //inputAction.Enable();
    }
    private void OnDisable()
    {
        //inputAction.Disable();
    }

    public void OnJump()
    {
        jumpInput = true;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        m_moveAmt = ctx.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        m_lookAmt = ctx.ReadValue<Vector2>();
    }

    void Update()
    {

        //rotationX += -UnityEngine.Input.GetAxis(camVertical) * sensitivity;
        rotationX += -m_lookAmt.y * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        //transform.rotation *= Quaternion.Euler(0, UnityEngine.Input.GetAxis(camHorizontal) * sensitivity, 0);
        transform.rotation *= Quaternion.Euler(0, m_lookAmt.x * sensitivity, 0);

/*        if (camVertical == "p2CamVer")
        {
            Debug.Log(-Input.GetAxis(camVertical));
            Debug.Log(-Input.GetAxis(camHorizontal));
        }*/

        if (!characterController.isGrounded)
        {
            verticalMovement.y -= gravity * Time.deltaTime;
        }
        else
        {
            verticalMovement.y = 0;
        }
        /*        if (inputAction. && characterController.isGrounded)
                {
                    verticalMovement.y = jumpforce;
                }*/
        /*        if (inputAction.GamePad.Jump.triggered && characterController.isGrounded)
                {
                    verticalMovement.y = jumpforce;
                }*/
        if (jumpInput && characterController.isGrounded)
        {
            verticalMovement.y = jumpforce;
        }

        //Vector3 direction = new Vector3(UnityEngine.Input.GetAxis(Horizontal), 0, UnityEngine.Input.GetAxis(Vertical));
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

        jumpInput = false;
    }
}
