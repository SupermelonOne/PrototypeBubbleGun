using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float sensitivity = 100;
    private float rotation = 0;
    private float realRotation = 0;

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    private void Update()
    {
        if (rotation != 0)
        {

        }
        if (rotation == -1)
        {
            if (realRotation > -2f)
            {
                realRotation -= 0.005f;
                realRotation *= 1.003f;
            }
        }
        else if (rotation == 1)
        {
            if (realRotation < 2f)
            {
                realRotation += 0.005f;
                realRotation *= 1.003f;
            }
        }
        else
        {
            realRotation = 0;
        }
        _camera.transform.Rotate(new Vector3(0, realRotation, 0) * sensitivity * Time.deltaTime);
    }

    public void leftTrue()
    {
        rotation = -1;
    }
    public void leftFalse()
    {
        rotation = 0;
    }
    public void rightTrue()
    {
        rotation = 1;
    }
    public void rightFalse()
    {
        rotation = 0;
    }
}
