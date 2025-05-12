using UnityEngine;
using UnityEngine.EventSystems;

public class CallFunctionOnEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse is over this UI element");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse left this UI element");
    }
}