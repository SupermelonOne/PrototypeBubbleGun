using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IndependantCallFunctionOnPlayerEnter : MonoBehaviour
{
    [SerializeField] private UnityEvent functions;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            functions.Invoke();
        }
    }
}
