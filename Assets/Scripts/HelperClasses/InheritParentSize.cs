using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InheritParentSize : MonoBehaviour
{
    [SerializeField] private Transform parentObj;

    private void Update()
    {
        transform.localScale = parentObj.localScale;
    }
}
