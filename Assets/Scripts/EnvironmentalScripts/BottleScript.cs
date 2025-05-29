using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BottleScript : MonoBehaviour
{
    [SerializeField] private GameObject sleepParticles;
    private List<MonsterMoveBehavior> monsters = new List<MonsterMoveBehavior>();
    [SerializeField] private int capacity = 1;
    private int amountFilled;

    private float clearTime = 5;
    private float clearTimer = 0;

    private void Start()
    {
        if (sleepParticles != null)
        {
            sleepParticles.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (capacity > amountFilled)
        {
            CatchScript catchScript = other.GetComponent<CatchScript>();
            if (catchScript != null && catchScript.caughtMonsters.Count > 0)
            {
                MonsterMoveBehavior moveBehavior = catchScript.caughtMonsters[0];
                if (moveBehavior != null && moveBehavior.isCaught)
                {
                    Monster monsterScript = moveBehavior.GetComponent<Monster>();
                    if (monsterScript != null && monsterScript.monsterCleanness != null && monsterScript.monsterCleanness.done)
                    {
                        moveBehavior.netPosition = transform;
                        monsters.Add(moveBehavior);
                        amountFilled++;
                        if (sleepParticles != null && !sleepParticles.activeSelf)
                        {
                            sleepParticles.SetActive(true);
                        }
                        catchScript.caughtMonsters.RemoveAt(0);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (monsters.Count > 0)
        {
            clearTimer += Time.deltaTime;
            if (clearTimer > clearTime)
            {
                clearTimer = 0;
                ClearBed();
            }
        }
    }
    
    private void ClearBed()
    {
        foreach(var monster in monsters)
        {
            Destroy(monster.gameObject);
            sleepParticles.SetActive(false);
        }
        monsters.Clear();
        amountFilled = 0;

    }
}
