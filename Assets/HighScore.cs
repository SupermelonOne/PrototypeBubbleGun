using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    TextMeshProUGUI text;
    string highscore;

    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        SetText();
    }

    private void SetText()
    {
        var scores = GameLoopManager.Instance.GetHighScores();
        highscore = "EL High scores:\n";
        for (int i = 0; i < scores.Length; i++)
        {
            highscore += $"{i+1}: {scores[i]} \n";
        }
        text.text = highscore;
    }
}
