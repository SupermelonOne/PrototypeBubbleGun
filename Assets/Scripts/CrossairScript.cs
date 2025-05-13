using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossairScript : MonoBehaviour
{
    private Vector3 raycastPosition;
    private float lastFireTime = -float.MaxValue;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Camera cam;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private float hideDistance = Mathf.Infinity;

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
                    Debug.Log("HIDE");
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + fireCooldown)
        {
            lastFireTime = Time.time;
            GameObject spawnedBullet = Instantiate(bullet);
            spawnedBullet.transform.position = cam.transform.position;
            MoveToTargetAndDestroy moveToTargetAndDestroy = spawnedBullet.AddComponent<MoveToTargetAndDestroy>();
            moveToTargetAndDestroy.targetPosition = raycastPosition;
        }
    }
}
