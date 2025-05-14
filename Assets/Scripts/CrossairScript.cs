using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossairScript : MonoBehaviour
{
    [SerializeField] private Transform bubbleSpawnPosition;
    private Vector3 raycastPosition;
    private float lastFireTime = -float.MaxValue;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Camera cam;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private float hideDistance = Mathf.Infinity;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

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
        if (hit.collider != null)
        {
            float distanceToHit = hit.distance;
            if (distanceToHit < hideDistance)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<MonsterMoveBehavior>().Hide();
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + fireCooldown)
        {
            lastFireTime = Time.time;
            GameObject spawnedBullet = Instantiate(bullet);
            if (bubbleSpawnPosition != null)
            {
                spawnedBullet.transform.position = bubbleSpawnPosition.position;
            }
            else
            {
                spawnedBullet.transform.position = cam.transform.position;
            }
            MoveToTargetAndDestroy moveToTargetAndDestroy = spawnedBullet.AddComponent<MoveToTargetAndDestroy>();
            // TODO dont do this but add curvature to bubble path instead
            //raycastPosition = ray.origin + ray.direction * 1000f;
            moveToTargetAndDestroy.targetPosition = raycastPosition;
        }
    }
}
