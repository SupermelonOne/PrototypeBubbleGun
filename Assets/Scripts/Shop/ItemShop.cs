using UnityEngine;

public class ItemShop : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private GameObject itemModel;
    [SerializeField] private int cost;
    [SerializeField] private float maxHeight;
    [SerializeField] private float hoverHeight;
    [SerializeField] private float rotationSpeed;
    
    [SerializeField] private Renderer rend;
    private GameObject obj;
    private void Start()
    {
        obj = Instantiate(itemModel, transform);
        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.tag = "Untagged";
        
        
        //this honestly shouldn't even be necessary unless people just throw stuff in here willie-nillie but lets just keep it here just in case
        foreach (var script in obj.GetComponentsInChildren<MonoBehaviour>())
            script.enabled = false;
        
        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
        
        maxHeight = Mathf.Max(0.1f, maxHeight);
        
        var itemRenderer  = obj.GetComponentInChildren<Renderer>();
        var xDiff = itemRenderer.bounds.extents.x / rend.bounds.extents.x;
        var yDiff = itemRenderer.bounds.extents.y / maxHeight;
        var zDiff = itemRenderer.bounds.extents.z / rend.bounds.extents.z;
        var largest = Mathf.Max(xDiff, yDiff, zDiff);
        
        obj.transform.localScale /= largest;
        
        Renderer[] scaledRenderers = obj.GetComponentsInChildren<Renderer>();
        Bounds scaledCombinedBounds = scaledRenderers[0].bounds;
        for (int i = 1; i < scaledRenderers.Length; i++)
        {
            scaledCombinedBounds.Encapsulate(scaledRenderers[i].bounds);
        }
        

        float desiredItemRendererMinY = rend.bounds.max.y + hoverHeight;
        float offsetFromObjPivotToRendererMinY = obj.transform.position.y - scaledCombinedBounds.min.y;
        float targetObjTransformY = desiredItemRendererMinY + offsetFromObjPivotToRendererMinY;

        obj.transform.position = new Vector3(rend.bounds.center.x, targetObjTransformY, rend.bounds.center.z);
    }
    
    

    private void Update()
    {
        var targetRotation = Quaternion.Euler(0, Time.time * rotationSpeed, 0);
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, targetRotation, Time.deltaTime);    
    }

    public void OnPlayerClicked(Player player)
    {
        if(player.inventory.ItemAmount(ItemType.Munny) >= cost)
            BuyItem(player.inventory, itemType, 1, cost);
    }

    private void BuyItem(PlayerInventory inventory, ItemType type, int amount, int price)
    {
        inventory.BuyItem(itemType, amount, price);
    }


}
