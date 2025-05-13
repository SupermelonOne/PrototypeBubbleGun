using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class StolenFpsController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!canMove) return;

        // Get input
        float vertical;
        float horizontal;
        if (Input.GetAxis("Vertical") >= 1)
        {
            vertical = 1f;
        }
        else if (Input.GetAxis("Vertical") <= -1)
        {
            vertical = -1f;
        }
        else
        {
            vertical = 0;
        }

        // TODO remove this controller and make a funny new one for the script
/*        if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -1f;
        }
        else*/
        {
            horizontal= 0;
        }
        float inputVertical = vertical;
        float inputHorizontal = horizontal;

        // Determine if running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runningSpeed : walkingSpeed;

        // Get camera's forward and right (flattened on Y axis)
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate move direction relative to camera
        Vector3 desiredMoveDirection = (cameraForward * inputVertical + cameraRight * inputHorizontal).normalized;

        // Preserve current Y velocity (for jumping/falling)
        float movementDirectionY = moveDirection.y;
        moveDirection = desiredMoveDirection * speed;
        moveDirection.y = movementDirectionY;

        // Jumping
        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }

        // Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);
    }
}