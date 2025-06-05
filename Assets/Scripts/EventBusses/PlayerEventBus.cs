using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventBus : BaseEventBus<PlayerEventBus>
{
    public class PlayerJoin
    {
        public Camera camera;
        public Transform transform;

        public PlayerJoin(Camera camera, Transform transform)
        {
            this.camera = camera;
            this.transform = transform;
        }
    }
}


