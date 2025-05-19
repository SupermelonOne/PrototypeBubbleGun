using UnityEngine;

public class MoveToTargetAndDestroy : MonoBehaviour
{
    public Vector3 targetPosition;
    public float speed = 15f;
    public float arcHeight = 5f;
    public float stopDistance = 0.1f;

    private Vector3 startPosition;
    private float journeyLength;
    private float startTime;

    void Start()
    {
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        startTime = Time.time;
        arcHeight = Mathf.Clamp((journeyLength / 5), 0.1f, 5);
    }

    void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;

        // Linear interpolation
        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

        // Add arc (parabolic offset in Y-axis)
        float heightOffset = arcHeight * 4 * fractionOfJourney * (1 - fractionOfJourney);
        currentPos.y += heightOffset;

        transform.position = currentPos;

        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            Destroy(gameObject);
        }
    }
}