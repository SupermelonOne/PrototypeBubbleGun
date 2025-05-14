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
            if (monster != null)
            {
                if (monster.isCaptured)
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
