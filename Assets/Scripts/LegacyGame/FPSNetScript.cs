using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSNetScript : MonoBehaviour
{
    [SerializeField] private Transform netTransform;
    [SerializeField] private float rotateModifier = 15f;
    [SerializeField] private string Vertical = "Vertical";
    [SerializeField] private int verticalAmp = 1;
    [SerializeField] private string Horizontal = "Horizontal";
    [SerializeField] private int horizontalAmp = 1;

    private void Start()
    {
        if (netTransform == null)
        {
            netTransform = transform;
        }
    }
    void Update()
    {
        float xRotation = Input.GetAxis(Vertical) * verticalAmp;
        float yRotation = Input.GetAxis(Horizontal) * horizontalAmp;
        netTransform.localRotation = Quaternion.Slerp(netTransform.localRotation, Quaternion.Euler(xRotation * 90, yRotation * 90, 0), Time.deltaTime * rotateModifier);
    }
}
