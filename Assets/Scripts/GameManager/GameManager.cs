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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddPoints(int amount)
    {
        Points += amount;
    }

    public void ResetPoints()
    {
        Points = 0;
    }
}

