using UnityEngine;

public class InventoryEventBus : BaseEventBus<ShopEventBus>
{
    public class OnNavigateUI
    {
        public InputTypes inputType;

        public OnNavigateUI(InputTypes inputType)
        {
            Debug.Log("nnnnaaaavigatin");
            this.inputType = inputType;
        }
    }
}