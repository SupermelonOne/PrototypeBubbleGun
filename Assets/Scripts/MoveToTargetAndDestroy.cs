using UnityEngine;

public class MoveToTargetAndDestroy : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 50f;
    public float stopDistance = 0.1f;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            Destroy(gameObject);
        }
    }
}