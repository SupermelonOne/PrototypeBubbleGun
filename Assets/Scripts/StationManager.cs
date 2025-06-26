using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    [SerializeField] private int stationCapacity = 1;
    [SerializeField] private StationScript monsterStorage;

    [SerializeField] private GameObject emptyGameObject;
    private List<Transform> monsterPositions = new List<Transform>();

    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private Transform exitPoint;

    //joris Im sorry too sstressed, we gotta link stuff in scene
    private List<MonsterCleanness> monsters = new List<MonsterCleanness>();
    private List<MonsterMoveBehavior> moveBehaviors = new List<MonsterMoveBehavior>();
    private List<GameObject> gameObjects = new List<GameObject>();

    private List<MonsterMoveBehavior> sendOutMoveBehaviors = new List<MonsterMoveBehavior>();
    private List<Transform> sendOutTransforms = new List<Transform>();
    //private List<GameObject> sendOutObjects = new List<GameObject>();


    public void AddMonster(MonsterCleanness newMonster)
    {
        monsters.Add(newMonster);
        MonsterMoveBehavior newMoveBehavior = newMonster.GetComponent<MonsterMoveBehavior>();
        Transform newTransform = new GameObject("monsterPos").transform;
        newMoveBehavior.netPosition = newTransform;
        newTransform.position = point1.position;
        moveBehaviors.Add(newMoveBehavior);
        
        gameObjects.Add(newMonster.gameObject);
        monsterPositions.Add(newTransform);

        newMonster.GetComponent<Monster>().EnterStation();
    }


    private void SendOut(MonsterCleanness pMonsterCleanness)
    {
        //this part might be obsolete lator

        MonsterMoveBehavior pMonsterMoveBehavior = pMonsterCleanness.GetComponent<MonsterMoveBehavior>();
        //this will stay
        sendOutMoveBehaviors.Add(pMonsterMoveBehavior);
        moveBehaviors.Remove(pMonsterMoveBehavior);

        monsters.Remove(pMonsterCleanness);
        //this is gonna bug out
        sendOutTransforms.Add(monsterPositions[0]);
        gameObjects.RemoveAt(0);
        pMonsterCleanness.GetComponent<Monster>().ExitStation();
    }


    private void Update()
    {
        if (monsters.Count > 0)
        {
            Debug.Log("monseter in it is: " + monsters[0].gameObject.name);
        }
        foreach (MonsterCleanness mon in monsters)
        {
            Debug.Log("monsters remaining");
            if (mon.done)
            {
                SendOut(mon);
                break;
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
        if (monsters.Count > 0)
        {
            if (!monsters[0].done)
            {
                if (Vector3.Distance(monsterPositions[0].position, point2.position) > 0.2f)
                {
                    monsterPositions[0].Translate((point2.position - monsterPositions[0].position).normalized * Time.deltaTime);
                }
            }
        }
        if (sendOutTransforms.Count > 0)
        {
            if (Vector3.Distance(sendOutTransforms[0].position, exitPoint.position) > 0.2f)
            {
                Debug.Log(sendOutTransforms[0].gameObject.name);
                sendOutTransforms[0].Translate((exitPoint.position - sendOutTransforms[0].position).normalized * Time.deltaTime);
            }
        }
    }


}
