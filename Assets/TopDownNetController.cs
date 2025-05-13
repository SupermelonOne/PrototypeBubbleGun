using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class TopDownNetController : MonoBehaviour
{
    [SerializeField] private GameObject shooterCamera;
    [SerializeField] private GameObject ELabel;
    [SerializeField] private float detectionRadius = 10;
    private float speed = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    Monster GetClosestEnemy()
    {
        var enemyLayer = LayerMask.GetMask("Monsters");
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        Monster closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider enemyCollider in enemies)
        {
            var monster = enemyCollider.gameObject.GetComponent<Monster>();

            float distance = Vector3.Distance(transform.position, enemyCollider.transform.position);
            
            if (distance < minDistance && monster != null)
            {
                minDistance = distance;
                closest = monster;
            }
        }

        return closest;
    }
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Get camera forward and right, but flatten the Y axis

        Vector3 camForward = Vector3.Scale(shooterCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(shooterCamera.transform.right, new Vector3(1, 0, 1)).normalized;

        // Calculate move direction
        Vector3 moveDirection = (camForward * vertical + camRight * horizontal).normalized;

        rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);

        var closeEnemy = GetClosestEnemy();
        if (closeEnemy != null && closeEnemy.isCaptured)
        {
            ELabel.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(closeEnemy.gameObject);
            }
        }
        else
        {
            ELabel.SetActive(false);
        }
    }
}
