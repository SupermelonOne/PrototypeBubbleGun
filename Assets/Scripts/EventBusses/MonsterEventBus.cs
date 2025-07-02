using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEventBus : BaseEventBus<MonsterEventBus>
{
    public class MonsterClean
    {
        public MonsterClean()
        {

        }
    }

    public class DirtCleaned
    {
        public ItemType type;
        public int amount;
        public DirtCleaned(int points, ItemType type)
        {
            this.amount = points;
            this.type = type;
        }
    }
}
