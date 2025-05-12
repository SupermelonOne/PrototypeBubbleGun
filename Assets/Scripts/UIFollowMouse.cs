using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowMouse : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform canvasRectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
    }
    private void Update()
    {
        MoveObject();
    }

    private void MoveObject()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,    
            Input.mousePosition,
            null,
            out pos
        );

        rectTransform.localPosition = pos;
    }

}
