using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGunScript : MonoBehaviour
{
    [SerializeField] private Transform bubbleSpawnPosition;
    private Vector3 raycastPosition;
    private float lastFireTime = -float.MaxValue;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Camera cam;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private float hideDistance = Mathf.Infinity;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private LayerMask layermask;
    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Update()
    {
        Ray ray = (cam.ScreenPointToRay(Input.mousePosition));
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layermask))
        {
            raycastPosition = hit.point;
        }
        else
        {
            // Default to a far point in the ray's direction if nothing is hit
            raycastPosition = ray.origin + ray.direction * 15f;
        }
        if (hit.collider != null)
        {
            float distanceToHit = hit.distance;
            if (distanceToHit < hideDistance)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<MonsterMoveBehavior>().Hide();
                    Debug.Log("raycasted on an enemy");
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            lastFireTime = Time.time;
            if (audioSource != null)
            {
                audioSource.Play();
                audioSource.pitch = Random.Range(0.8f, 1.2f);
            }
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

        if (bubbleSpawnPosition != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(ray.direction);
            bubbleSpawnPosition.rotation = Quaternion.Slerp(bubbleSpawnPosition.rotation, targetRotation, 15f * Time.deltaTime);
        }
    }
}
