using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wasd : MonoBehaviour
{
    Rigidbody rb;
    public float modifier = 10;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (UnityEngine.Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector3.up * modifier * Time.deltaTime);
        }
        if (UnityEngine.Input.GetKey(KeyCode.S))
        {
            rb.AddForce(Vector3.down * modifier * Time.deltaTime);
        }

        if (UnityEngine.Input.GetKey(KeyCode.A))
        {
            rb.AddForce(new Vector3(0, 0, 1) * modifier * Time.deltaTime);
        }
        if (UnityEngine.Input.GetKey(KeyCode.D))
        {
            rb.AddForce(new Vector3(0, 0, -1) * modifier *   Time.deltaTime);
        }
        if (!UnityEngine.Input.GetKey(KeyCode.D) && !UnityEngine.Input.GetKey(KeyCode.A) && !UnityEngine.Input.GetKey(KeyCode.W) && !UnityEngine.Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector3 (0, 0, 0);
        }
    }
}
