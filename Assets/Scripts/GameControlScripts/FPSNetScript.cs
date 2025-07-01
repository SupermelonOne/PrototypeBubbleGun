using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSNetScript : PlayerAction
{
    [SerializeField] private Transform netTransform;
    [SerializeField] private float rotateModifier = 15f;
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
        float xRotation = m_moveAmt.y; // * verticalAmp
        float yRotation = m_moveAmt.x; // * horizontalAmp
        netTransform.localRotation = Quaternion.Slerp(netTransform.localRotation, Quaternion.Euler(xRotation * 90, yRotation * 90, 0), Time.deltaTime * rotateModifier);
    }
}
