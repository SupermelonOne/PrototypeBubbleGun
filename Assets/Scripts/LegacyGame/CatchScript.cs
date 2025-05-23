using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchScript : MonoBehaviour
{
    List<MonsterMoveBehavior> caughtMonsters = new List<MonsterMoveBehavior>();
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
                    Debug.Log("should be captured");
                }
                moveBehavior.Capture(transform);
                if (!caughtMonsters.Contains(moveBehavior))
                {
                    caughtMonsters.Add(moveBehavior);
                }
            }
        }
    }
    void Update()
    {
        //change this to controller input
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach(var monster in caughtMonsters)
            {
                monster.Release();
            }
        }
    }
}
