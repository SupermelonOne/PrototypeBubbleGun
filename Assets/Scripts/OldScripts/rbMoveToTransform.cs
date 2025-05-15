using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rbMoveToTransform : MonoBehaviour
{
    public Transform target;
    public float force = 5;
    private Vector3 direction = Vector3.zero;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = (target.position - transform.position).normalized;
        if (rb.velocity.x > 0 && direction.x < 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
        else if (rb.velocity.x < 0 && direction.x > 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
        rb.AddForce(direction * force * Time.deltaTime);
    }
}
