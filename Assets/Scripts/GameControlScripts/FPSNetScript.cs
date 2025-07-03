using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSNetScript : PlayerAction
{
    [SerializeField] private Transform netTransform;
    [SerializeField] private float rotateModifier = 15f;
    private Quaternion netRotation = Quaternion.identity;
    private Quaternion initialRotationOffset = Quaternion.identity; // <-- for syncing
    //[SerializeField] private string Vertical = "Vertical";
    //[SerializeField] private int verticalAmp = 1;
    //[SerializeField] private string Horizontal = "Horizontal";
    //[SerializeField] private int horizontalAmp = 1;

    private Vector2 m_moveAmt = Vector2.zero;

    public void OnMove(InputAction.CallbackContext ctx)
    {
        m_moveAmt = ctx.ReadValue<Vector2>();
    }

    protected override void OnMonsterCast(RaycastHit hit)
    {
        //throw new System.NotImplementedException();
    }

    private void Start()
    {
        if (netTransform == null)
        {
            netTransform = transform;
        }
    }
    void Update()
    {


        if (inputManager != null)
        {
            Vector3 angularVelocity = inputManager.gyroscope; // in degrees per second
            netRotation *= Quaternion.Euler(angularVelocity * Time.deltaTime);

            // Sync orientation when Button 1 is pressed
            if (inputManager._button1_pressed && inputManager._button2_hold)
            {
                // Save the current rotation as the "forward" offset
                initialRotationOffset = Quaternion.Inverse(netRotation);
                Debug.Log("Synced orientation.");
            }

            // Apply synced offset
            netTransform.localRotation = initialRotationOffset * netRotation;
        }
        else
        {
            float xRotation = m_moveAmt.y;
            float yRotation = m_moveAmt.x;
            netTransform.localRotation = Quaternion.Slerp(
                netTransform.localRotation,
                Quaternion.Euler(xRotation * 90, yRotation * 90, 0),
                Time.deltaTime * rotateModifier
            );
        }
    }
}
