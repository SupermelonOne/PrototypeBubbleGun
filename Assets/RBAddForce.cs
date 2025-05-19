using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBAddForce : MonoBehaviour
{
    [SerializeField] private Vector3 addforcething;
    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(addforcething, ForceMode.Impulse);
    }
}
