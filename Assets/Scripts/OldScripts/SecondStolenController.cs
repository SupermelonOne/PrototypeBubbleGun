using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondStolenController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speedModifier = 2;
    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        Debug.Log("Horizontal is " + Input.GetAxis("Horizontal"));
        float rememberY = rb.velocity.y;
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (direction.magnitude > 1)
        {
            direction.Normalize();
        }
        Debug.Log(direction.x);

        direction *= speedModifier;
        direction.y = rememberY;

        rb.velocity = direction;
    }
}
