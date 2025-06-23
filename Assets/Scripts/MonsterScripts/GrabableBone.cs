using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GrabableBone : MonoBehaviour
{
    [HideInInspector] public bool pulling = false;
    [SerializeField] private GrabableBone parentBone;
    [SerializeField] private float parentIntensity = 0.5f;

    [SerializeField] private bool leftArm = false;
    [SerializeField] private bool rightArm = false;
    [SerializeField] private bool leftLeg = false;
    [SerializeField] private bool rightLeg = false;
    private Vector3 origin;
    private Transform glovePosition;
    private GloveScript currentGlove;

    public Vector3 up;
    public Vector3 forward;

    private Vector3 activePosition;
    private Vector3 pullPosition;
    private GameObject activePlane;
    [SerializeField] private GameObject plane;

    [SerializeField] private float allowedDistance = 0.5f;
    private void Start()
    {
        origin = transform.position;
        activePosition = origin;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Glove"))
        {
            GloveScript gloveScript = other.GetComponent<GloveScript>();
            if (gloveScript == null)
                return;
            if (!gloveScript.holding && gloveScript.grabbing)
            {
                gloveScript.Grab();

                // SPAWN PLANE TO MOVE MORE EASILY
                activePlane = Instantiate(plane);
                activePlane.transform.position = transform.position;
                activePlane.transform.forward = (transform.position - gloveScript.GetOrigin()).normalized;

                glovePosition = gloveScript.transform;
                currentGlove = gloveScript;
            }
        }
    }
    private void Update()
    {
        
    }
    private void LateUpdate()
    {
        if (currentGlove == null || glovePosition == null)
        {
            if (pulling)
            {
                origin = transform.position;
                pullPosition = transform.position;
                activePosition = pullPosition;
            }
        }
        else
        {
            Pull(currentGlove, glovePosition, 1);
            if (!currentGlove.grabbing)
            {
                currentGlove.Release();
                Destroy(activePlane);
                currentGlove = null;
                glovePosition = null;
            }
        }
        pulling = false;
    }

    public void Pull(GloveScript pCurrentGlove, Transform pGlovePosition, float intensity)
    {
        if (rightArm)
        {
            Vector3 grabDirection = (pGlovePosition.position - origin).normalized;

            Vector3 up = (pGlovePosition.position - origin).normalized;

            Vector3 arbitrary = Mathf.Abs(Vector3.Dot(up, Vector3.up)) > 0.99f ? Vector3.right : Vector3.up;

            Vector3 forward = Vector3.Cross(arbitrary, up).normalized;
            Vector3 right = Vector3.Cross(up, forward).normalized; // Optional: also use to debug

            Quaternion rotation = Quaternion.LookRotation(forward, up);


            rotation *= Quaternion.Euler(0, 90f, 0);


            transform.rotation = rotation;
        }
        else if (leftArm)
        {
            Vector3 grabDirection = (pGlovePosition.position - origin).normalized;

            Vector3 up = (pGlovePosition.position - origin).normalized;

            Vector3 arbitrary = Mathf.Abs(Vector3.Dot(up, Vector3.up)) > 0.99f ? Vector3.right : Vector3.up;

            Vector3 forward = Vector3.Cross(arbitrary, up).normalized;
            Vector3 right = Vector3.Cross(up, forward).normalized; // Optional: also use to debug

            Quaternion rotation = Quaternion.LookRotation(forward, up);

            rotation *= Quaternion.Euler(0, -90f, 0);


            transform.rotation = rotation;
        }
        else if (leftLeg)
        {
            Vector3 up = (pGlovePosition.position - origin).normalized;

            // Use a consistent world-space forward direction to prevent twisting
            Vector3 referenceForward = Vector3.forward;

            // Project it onto the plane perpendicular to 'up'
            Vector3 projectedForward = Vector3.ProjectOnPlane(referenceForward, up).normalized;

            // Construct stable rotation with no twist
            Quaternion rot = Quaternion.Euler(0, -90, 0);
            Quaternion rotation = Quaternion.LookRotation(-projectedForward, up);

            transform.rotation = rotation;
        }


        //I first wanna fix rotation stuff
        if (Vector3.Distance(origin, pGlovePosition.position) * intensity > allowedDistance * intensity)
        {
            pullPosition = (pGlovePosition.position - origin).normalized * allowedDistance * intensity + origin;
        }
        else
        {
            pullPosition = pGlovePosition.position;
        }
        up = transform.up;
        forward = transform.forward;


        transform.position = activePosition;
        activePosition = Vector3.Lerp(activePosition, pullPosition, 10 * Time.deltaTime);

        if (parentBone == null) return;
        //parentBone.pulling = true;
        //parentBone.Pull(pCurrentGlove, pGlovePosition, parentIntensity);
    }
}
