using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private void Start()
    {
        if (cam == null)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }

    void Update()
    {
        // Find the player using the "Player" tag
        if (cam != null)
        {
            // Make the popup face the player
            Vector3 pos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
            this.transform.LookAt(pos);
            this.transform.Rotate(0, 180, 0); // Optional: Rotate 180 degrees if the text is backward
        }
    }
}
