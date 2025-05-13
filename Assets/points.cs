using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Points : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public static Points Instance { get; private set; }
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional: keep across scenes
    }
    void Start()
    {
        UpdateScoreText();
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    public void AddPoints(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Points: " + score.ToString();
        }
    }
}