using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootBubble : PlayerAction
{
    [SerializeField] private Transform bubbleSpawnPosition;
    private float lastFireTime = -float.MaxValue;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float fireCooldown = 0.5f;
    [SerializeField] private float hideDistance = Mathf.Infinity;


    public override void ButtonDown()
    {
        if (bubbleSpawnPosition != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cam.transform.forward);
            bubbleSpawnPosition.rotation = Quaternion.Slerp(bubbleSpawnPosition.rotation, targetRotation, 15f * Time.deltaTime);
        }
    }

    public override void StartShooting()
    {
        if (Time.time >= lastFireTime + fireCooldown)
        {
            lastFireTime = Time.time;
            
            //audioSource cannot be null
            audioSource.Play();
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            
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

    public override void OnMonsterCast(RaycastHit hit)
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
}
