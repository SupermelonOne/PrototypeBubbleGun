using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MouseOverUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UnityEvent onMouseEnter;
    [SerializeField] private UnityEvent onMouseExit;
    public void OnPointerEnter(PointerEventData eventData)
    {
        onMouseEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onMouseExit.Invoke();
    }
}