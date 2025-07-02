using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoopManager : MonoBehaviour
{
    //variables
    [SerializeField] private int minutes;
    [SerializeField] private int seconds;
    [SerializeField] private int highScoreAmount;

    private int initialMinutes;
    private int initialSeconds;

    //c# events so everything can subscribe without editor bs
    public bool menuMode = true;
    public bool dead = false;
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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // save initial values
        initialMinutes = minutes;
        initialSeconds = seconds;

        //creating the file
        path = Path.Combine(Application.persistentDataPath, "highscores.txt");
        if (!File.Exists(path))
            SaveHighScores();
    }

    void Update()
    {
        if(Input.anyKeyDown && (menuMode || dead))
            SwitchScene();
        
        if (menuMode || dead)
            return;

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
            SwitchScene();
            return;
        }
            
        OnSecondPassed?.Invoke(minutes, seconds);

        timer = 0f;
    }

    private void SwitchScene()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        if (dead)
        {
            menuMode = true;
            dead = false;
            timer = 0f;
            minutes = initialMinutes;
            seconds = initialSeconds;
        }
        else if (menuMode)
        {
            menuMode = false;
            dead = false;
        }
        else
        {
            menuMode = false;
            dead = true;
        }
    }

    public int[] GetHighScores()
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
        scoreList.RemoveAt(0);
        scoreList.Reverse();
        highScores = scoreList.ToArray();
        for (var i = 0; i < highScores.Length; i++)
        {
            Debug.Log(highScores[i]);
        }
        SaveHighScores();
    }

    private void SaveHighScores()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
            {
                for (int i = 0; i < highScoreAmount; i++)
                {
                    var s = (highScores != null && i < highScores.Length) ? highScores[i] : 0;
                    sw.WriteLine(s);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save high scores: " + e.Message);
        }
    }
}
