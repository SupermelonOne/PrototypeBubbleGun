using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private List<Transform> cameras = new List<Transform>();

    private void Start()
    {
        List<Camera> cameraObjs = FindObjectsOfType<Camera>().ToList<Camera>();
        foreach (var camera in cameraObjs)
        {
            cameras.Add(camera.transform);
        }
    }

    void Update()
    {
        if (cameras.Count > 0)
        {
            float distanceToCam = Mathf.Infinity;
            foreach (var camera in cameras)
            {
                if (distanceToCam < Vector3.Distance(camera.position, transform.position))
                {
                    cam = camera.GetComponent<Camera>();
                }
            }
        }
        if (cam != null)
        {
            Vector3 pos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
            this.transform.LookAt(pos);
            this.transform.Rotate(0, 180, 0); // Optional: Rotate 180 degrees if the text is backward
        }
    }
}
