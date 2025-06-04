using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All purpose GameManager script, if this gets too full we should split it up
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Points { get; private set; }

    private void OnEnable()
    {
        MonsterEventBus.Subscribe<MonsterEventBus.DirtClean>(OnDirtClean);
        MonsterEventBus.Subscribe<MonsterEventBus.MonsterClean>(OnMonsterClean);
    }
    private void OnDisable()
    {
        MonsterEventBus.Subscribe<MonsterEventBus.DirtClean>(OnDirtClean);
        MonsterEventBus.Subscribe<MonsterEventBus.MonsterClean>(OnMonsterClean);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDirtClean(MonsterEventBus.DirtClean dirtClean)
    {
        AddPoints(dirtClean.points);
    }
    private void OnMonsterClean(MonsterEventBus.MonsterClean monsterClean)
    {
        AddPoints(monsterClean.points);
    }

    public void AddPoints(int amount)
    {
        Points += amount;
        Debug.Log(Points);
        UIEventBus.Invoke(new UIEventBus.UpdateScore(Points));
    }

    public void ResetPoints()
    {
        Points = 0;
    }


}

