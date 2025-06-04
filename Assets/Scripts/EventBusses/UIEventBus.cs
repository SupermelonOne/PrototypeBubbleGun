using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventBus : BaseEventBus<UIEventBus>
{
    public class UpdateScore
    {
        public int score;
        public string scoreText;

        public UpdateScore(int newScore)
        {
            score = newScore;
            scoreText = score.ToString();
        }
    }
}
