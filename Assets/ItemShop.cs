using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ItemShop : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private float maxHeight;
    [SerializeField] private float hoverHeight;
    [SerializeField] private float rotationSpeed;
    private Renderer rend;
    private GameObject obj;
    private void Start()
    {
        obj = Instantiate(item, transform);
        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.tag = "Untagged";
        
        //this honestly shouldnt even be necessary unless people just throw stuff in here willie-nillie but lets just keep it here just in case
        foreach (var script in obj.GetComponentsInChildren<MonoBehaviour>())
            script.enabled = false;
        
        foreach (var rb in obj.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;

        rend = GetComponentInChildren<Renderer>();
        
        maxHeight = Mathf.Max(0.1f, maxHeight);
        
        
        var itemRenderer  = obj.GetComponentInChildren<Renderer>();
        var xDiff = itemRenderer.bounds.extents.x / rend.bounds.extents.x;
        var yDiff = itemRenderer.bounds.extents.y / maxHeight;
        var zDiff = itemRenderer.bounds.extents.z / rend.bounds.extents.z;
        var largest = Mathf.Max(xDiff, yDiff, zDiff);
        
        Debug.Log($"Largest: {largest}");
        
        obj.transform.localScale /= largest;
        
        var position = rend.bounds.max.y + obj.GetComponentInChildren<Renderer>().bounds.extents.y + hoverHeight;
        var pos = new Vector3(0, position, 0);
        
        obj.transform.position += pos;
    }

    private void Update()
    {
        var targetRotation = Quaternion.Euler(0, Time.time * rotationSpeed, 0);
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, targetRotation, Time.deltaTime);    
    }
}
