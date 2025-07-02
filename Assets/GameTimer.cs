using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private void OnEnable()
    {
        GameLoopManager.Instance.OnSecondPassed += SetTime;
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnDisable()
    {
        GameLoopManager.Instance.OnSecondPassed -= SetTime;
    }

    private void SetTime(int min, int sec)
    {
        timerText.text = $"{min:00}:{sec:00}";
    }
}
