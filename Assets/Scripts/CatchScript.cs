using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchScript : MonoBehaviour
{

    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monster monster = other.GetComponent<Monster>();
            MonsterMoveBehavior moveBehavior = other.GetComponent<MonsterMoveBehavior>();
            if (monster != null && moveBehavior != null)
            {
                if (monster.isCaptured && !moveBehavior.isCaught)
                {
                    monster.PutInNet(transform);
                    Debug.Log("should be captured");
                }
            }
        }
    }
    void Update()
    {

    }
}
