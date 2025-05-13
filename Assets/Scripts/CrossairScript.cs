using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossairScript : MonoBehaviour
{
    private Vector3 raycastPosition;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Camera cam;

    private void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);
        if (Physics.Raycast(ray, out hit))
        {
            raycastPosition = hit.point;
        }
        else
        {
            // Default to a far point in the ray's direction if nothing is hit
            raycastPosition = ray.origin + ray.direction * 1000f;
        }

        if (Input.GetMouseButtonDown(0))
        {
            GameObject spawnedBullet = Instantiate(bullet);
            spawnedBullet.transform.position = cam.transform.position;
            MoveToTargetAndDestroy moveToTargetAndDestroy = spawnedBullet.AddComponent<MoveToTargetAndDestroy>();
            moveToTargetAndDestroy.targetPosition = raycastPosition;
        }
    }
}