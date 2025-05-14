using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            MonsterMoveBehavior moveBehavior = other.GetComponent<MonsterMoveBehavior>();
            if (moveBehavior != null )
            {
                moveBehavior.netPosition = transform;
            }
        }
    }
}
