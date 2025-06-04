using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEventBus : BaseEventBus<MonsterEventBus>
{
    public class MonsterClean
    {
        public int points;
        public MonsterClean(int points)
        {
            this.points = points;
        }
    }
    public class DirtClean
    {
        public int points;
        public DirtClean(int points)
        {
            this.points = points;
        }
    }
}
