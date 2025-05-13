using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSNetScript : MonoBehaviour
{
    [SerializeField] private Transform netTransform;
    private void Start()
    {
        if (netTransform == null)
        {
            netTransform = transform;
        }
    }
    void Update()
    {
        float xRotation = Input.GetAxis("Vertical");
        float yRotation = Input.GetAxis("Horizontal");
        netTransform.localRotation = Quaternion.Euler(xRotation * 90, yRotation * 90, 0);
        Debug.Log("xRotation = " + xRotation);
        Debug.Log("zRotation = " + yRotation);
    }
}
