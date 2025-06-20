using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private Texture2D _dirtMaskBase;
    [SerializeField] private Texture2D _brush;

    [SerializeField] private Material _material;

    private Texture2D _templateDirtMask;

    private void Start()
    {
        CreateTexture();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {

        }
    }

    private void CreateTexture()
    {
        _templateDirtMask = new Texture2D(_dirtMaskBase.width, _dirtMaskBase.height);
        _templateDirtMask.SetPixels(_dirtMaskBase.GetPixels());
        _templateDirtMask.Apply();

        _material.SetTexture("DirtTexture", _templateDirtMask);
    }
}