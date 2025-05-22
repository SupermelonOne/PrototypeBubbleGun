using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecondStolenController : MonoBehaviour
{ 
    [SerializeField] private string Horizontal = "Horizontal";
    [SerializeField] private string Vertical = "Vertical";

    private Vector3 verticalMovement = Vector3.zero;

    private CharacterController characterController;
    [SerializeField] private float speedModifier = 2;
    [SerializeField] Camera playerCamera;
    [SerializeField] private float sensitivity = 2.0f;
    [SerializeField] private float gravity  = 20.0f;
    public float lookXLimit = 45.0f;
    float rotationX = 0;
    [SerializeField] private float jumpforce = 5;

    private void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }
    void Update()
    {
        rotationX += -Input.GetAxis("Mouse Y") * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * sensitivity, 0);

        if (!characterController.isGrounded)
        {
            verticalMovement.y -= gravity * Time.deltaTime;
        }
        else
        {
            verticalMovement.y = 0;
        }
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            verticalMovement.y = jumpforce;
        }


        Vector3 direction = new Vector3(Input.GetAxis(Horizontal), 0, Input.GetAxis(Vertical));
        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }
        Vector3 desiredDirection = (direction.z * playerCamera.transform.forward + direction.x * playerCamera.transform.right);
        desiredDirection *= speedModifier;



        desiredDirection += verticalMovement;
        characterController.Move(desiredDirection * Time.deltaTime);
    }
}
