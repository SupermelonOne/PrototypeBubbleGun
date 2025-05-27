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
                catchScript.caughtMonsters.RemoveAt(0);
                if (moveBehavior.isCaught && moveBehavior != null)
                {
                    moveBehavior.netPosition = transform;
                    monsters.Add(moveBehavior);
                    amountFilled++;
                    Debug.Log("monster should sleep now");
                    if (sleepParticles != null && !sleepParticles.activeSelf)
                    {
                        sleepParticles.SetActive(true);
                    }

                }
            }
        }
    }

    private void Update()
    {

    }
}
