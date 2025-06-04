using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    void Start()
    {
        if (displayText == null)
        {
            GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        UIEventBus.Subscribe<UIEventBus.UpdateScore>(UpdateText);
    }
    private void OnDisable()
    {
        UIEventBus.UnSubscribe<UIEventBus.UpdateScore>(UpdateText);
    }

    private void UpdateText(UIEventBus.UpdateScore updateScore)
    {
        ChangeText(updateScore.scoreText);
    }
    private void ChangeText(string updatedText)
    {
        displayText.text = updatedText;
    }
}
