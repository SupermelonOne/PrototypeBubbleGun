using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameLoopManager : MonoBehaviour
{
    //variables
    [SerializeField] private int minutes;
    [SerializeField] private int seconds;
    [SerializeField] private int highScoreAmount;

    //c# events so everything can subscribe without editor bs
    public event Action OnGameOver;
    public event Action<int, int> OnSecondPassed;
    
    private float timer;
    private int [] highScores;
    private string path;
    
    public static GameLoopManager Instance;

    void Awake()
    {
        //awesome singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //creating the file
        path = Path.Combine(Application.persistentDataPath, "highscores.txt");
        if (!File.Exists(path))
            SaveHighScores();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!(timer >= 1f)) return;
        
        if (seconds > 1)
        {
            seconds--;
        }
        else if (minutes > 0)
        {
            minutes--;
            seconds = 59;
        }
        else
        {
            AddHighScore(PlayerInventory.Instance.ItemAmount(ItemType.Munny));
            OnGameOver?.Invoke();
            return;
        }
            
        OnSecondPassed?.Invoke(minutes, seconds);

        timer = 0f;
    }

    private int[] GetHighScores()
    {
        highScores = new int [highScoreAmount];
        try
        {
            var sr = new StreamReader(path);
            for (var i = 0; i < highScoreAmount; i++)
            {
                int.TryParse(sr.ReadLine(), out var score);
                highScores[i] = score;
            }
            sr.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        return highScores;
    }

    private void AddHighScore(int score)
    {
        var scoreList = highScores.ToList();
        scoreList.Add(score);
        scoreList.Sort();
        scoreList.RemoveAt(scoreList.Count - 1);
        highScores = scoreList.ToArray();
        SaveHighScores();
    }

    private void SaveHighScores()
    {
        StreamWriter sw = new StreamWriter(path);
        
        for (int i = 0; i < highScoreAmount; i++)
        {
            var s = highScores[i];
            sw.WriteLine(s);
        }
    }
}
