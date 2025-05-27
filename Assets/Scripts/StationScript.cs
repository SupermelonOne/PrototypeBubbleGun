using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationScript : MonoBehaviour
{
    private MonsterCleanness caughtMonster;
    private MonsterMoveBehavior moveBehavior;
    //private MonsterCleanness caughtMonsterCleanness;
    private void OnTriggerEnter(Collider other)
    {
        CatchScript catchScript = other.GetComponent<CatchScript>();
        if (catchScript != null && catchScript.caughtMonsters.Count > 0 && caughtMonster == null)
        {
            MonsterMoveBehavior monster = catchScript.caughtMonsters[0];
            catchScript.caughtMonsters.RemoveAt(0);
            if (monster != null && monster.isCaught && caughtMonster == null)
            {
                monster.EnterStation();
                monster.netPosition = transform;
                caughtMonster = monster.GetComponent<MonsterCleanness>();
                moveBehavior = monster;
            }
        }
    }

    private void Update()
    {
        if (caughtMonster == null)
        {

        }
        if (caughtMonster != null)
        {
            if (caughtMonster.done)
            {
                caughtMonster = null;
                if (moveBehavior != null)
                {
                    moveBehavior.Release();
                }
            }
        }
    }
}
