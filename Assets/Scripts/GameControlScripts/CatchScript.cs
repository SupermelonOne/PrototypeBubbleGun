using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchScript : MonoBehaviour
{
    [HideInInspector] public List<MonsterMoveBehavior> caughtMonsters = new List<MonsterMoveBehavior>();
    [SerializeField] private int capacity = 1;
    private bool allowCatching = false;
    void Start()
    {

    }

    private void OnEnable()
    {
        foreach(MonsterMoveBehavior monster in caughtMonsters)
        {
            monster.gameObject.SetActive(true);
        }
    }
    private void OnDisable()
    {
        foreach (MonsterMoveBehavior monster in caughtMonsters)
        {
            monster.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (allowCatching && caughtMonsters.Count < capacity)
        {
            if (other.CompareTag("Enemy"))
            {
                Monster monster = other.GetComponent<Monster>();
                MonsterMoveBehavior moveBehavior = other.GetComponent<MonsterMoveBehavior>();
                if (monster != null && moveBehavior != null && !moveBehavior.isCaught)
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
    }
    void Update()
    {
        //change this to controller input
        if (UnityEngine.Input.GetKeyDown(KeyCode.E))
        {
            foreach(var monster in caughtMonsters)
            {
                monster.Release();
            }
            caughtMonsters.Clear(); 
        }
        if (true) // TODO add check for if this player is holding down move button, if so, allow to catch, than if it catches smthng in the same button press, disable r smthng
        {
            allowCatching = true;
        }    
    }
}
