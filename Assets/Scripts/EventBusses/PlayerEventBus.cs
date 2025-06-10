using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventBus : BaseEventBus<PlayerEventBus>
{
    public class PlayerJoin
    {
        public Camera camera;
        public Player player;

        public PlayerJoin(Camera camera, Player player)
        {
            this.camera = camera;
            this.player = player;
        }
    }
}


