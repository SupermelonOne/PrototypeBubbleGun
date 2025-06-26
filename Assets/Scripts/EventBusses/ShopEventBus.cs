using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum InputTypes
{
    None,
    Up,
    Down,
    Select,
    Back
}

public class ShopEventBus : BaseEventBus<ShopEventBus>
{
    public class OnShopActivated
    {
        public Player player;

        public OnShopActivated(Player player)
        {
            this.player = player;
        }
    }

    public class OnNavigateUI
    {
        public InputTypes inputType;

        public OnNavigateUI(InputTypes inputType)
        {
            this.inputType = inputType;
        }
    }
}



