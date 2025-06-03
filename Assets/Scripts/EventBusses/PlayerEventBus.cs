using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventBus : BaseEventBus<PlayerEventBus>
{
    public class PlayerJoin
    {
        public Camera camera;

        public PlayerJoin(Camera camera)
        {
            this.camera = camera;
        }
    }
}


