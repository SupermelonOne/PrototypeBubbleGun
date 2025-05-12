using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMouse : MonoBehaviour
{
    public LayerMask layerMask;
    Vector3 worldPosition;
    Vector3 screenPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Placing();
    }

    void Placing()
    {
        screenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hitData, 100, layerMask))
        {
            worldPosition = hitData.point;
        }
        transform.position = worldPosition;
    }
}
