using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class GrabableBone : MonoBehaviour
{
    [SerializeField] private bool arm = false;
    [SerializeField] private bool leg = false;
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
            origin = transform.position;
            pullPosition = transform.position;
        }
        else
        {


            if (arm)
            {
                Vector3 grabDirection = (glovePosition.position - origin).normalized;

                Vector3 up = (glovePosition.position - origin).normalized;

                Vector3 arbitrary = Mathf.Abs(Vector3.Dot(up, Vector3.up)) > 0.99f ? Vector3.right : Vector3.up;

                Vector3 forward = Vector3.Cross(arbitrary, up).normalized;
                Vector3 right = Vector3.Cross(up, forward).normalized; // Optional: also use to debug

                Quaternion rotation = Quaternion.LookRotation(forward, up);
                if (arm)
                {
                    rotation *= Quaternion.Euler(0, 90f, 0);
                }

                transform.rotation = rotation;
            }
            if (leg)
            {
                Vector3 grabDirection = (glovePosition.position - origin).normalized;


                Quaternion rotation = Quaternion.Euler(0, -90, 0);
                Vector3 newForward = rotation * (currentGlove.origin.position - transform.position);
                Quaternion rot = Quaternion.LookRotation(newForward, grabDirection);
                transform.rotation = rot;
            }


            //I first wanna fix rotation stuff
/*            if (Vector3.Distance(origin, glovePosition.position) > allowedDistance)
            {
                pullPosition = (glovePosition.position - origin).normalized * allowedDistance + origin;
            }
            else
            {
                pullPosition = glovePosition.position;
            }*/
            up = transform.up;
            forward = transform.forward;
            if (!currentGlove.grabbing)
            {
                currentGlove.Release();
                Destroy(activePlane);
                currentGlove = null;
                glovePosition = null;
            }
        }

        transform.position = activePosition;
        activePosition = Vector3.Lerp(activePosition, pullPosition, 10 * Time.deltaTime);


    }
}
