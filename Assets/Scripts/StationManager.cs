using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    [SerializeField] private int stationCapacity = 1;
    [SerializeField] private StationScript monsterStorage;

    //joris Im sorry too sstressed, we gotta link stuff in scene
    private List<MonsterCleanness> monsters = new List<MonsterCleanness>();
    private List<GameObject> gameObjects = new List<GameObject>();

    public void AddMonster(MonsterCleanness newMonster)
    {
        monsters.Add(newMonster);
        gameObjects.Add(newMonster.gameObject);
    }


    private void SendOut(MonsterCleanness pMonsterCleanness)
    {
        //this part might be obsolete lator
        MonsterMoveBehavior moveBehavior = pMonsterCleanness.GetComponent<MonsterMoveBehavior>();
        if (moveBehavior != null)
        {
            moveBehavior.ExitStation();
            moveBehavior.Release();
            moveBehavior.transform.position = monsterStorage.transform.position;
        }
            
        //this will stay
        monsters.Remove(pMonsterCleanness);
        gameObjects.Remove(pMonsterCleanness.gameObject);        
    }


    private void Update()
    {
        foreach(MonsterCleanness mon in monsters)
        {
            if (mon.done)
            {
                SendOut(mon);
            }
        }
        if (monsters.Count < stationCapacity)
        {
            MonsterCleanness checkForMonster = monsterStorage.GetFirstInQueue();
            if (checkForMonster != null)
            {
                AddMonster(checkForMonster);
            }
        }
    }


}
