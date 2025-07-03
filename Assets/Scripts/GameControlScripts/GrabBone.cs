using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabBone : PlayerAction
{
    [SerializeField] private List<Transform> glove = new List<Transform>();
    [SerializeField] private Transform origin;
    private Vector3 realDestination;
    private Vector3 hitPoint;
    [SerializeField] private GloveScript gloveScript;

    private Vector3 targetPosition = Vector3.zero;

    private void Start()
    {
        if (gloveScript == null)
            gloveScript = glove[0].GetComponent<GloveScript>();
        if (gloveScript == null)
            return;
        gloveScript.SetOrigin(transform);
    }

    protected override void ButtonDown()
    {
        if (glove == null) return;

        hitPoint = raycastPosition;

        realDestination = hitPoint;

        foreach(Transform t in glove)
        {
            //t.position = Vector3.Lerp(t.position, realDestination, Time.deltaTime * 10f);

        }

        if (gloveScript == null)
            return;
        gloveScript.Activate();
    }
    protected override void StopShooting()
    {
        if (gloveScript == null)
            return;
        gloveScript.DeActivate();
    }

    protected override void PassiveUpdate()
    {
        if (!holding)
        {
            foreach(Transform t in glove)
            {
                //t.position = Vector3.Lerp(t.position, origin.position, Time.deltaTime * 10f);
            }
        }
        if (holding)
        {

            foreach (Transform t in glove)
            {
                t.position = Vector3.Lerp(t.position, realDestination, Time.deltaTime * 10f);

            }
        }
    }

    protected override void OnMonsterCast(RaycastHit hit)
    {
        hitPoint = hit.point;
    }
}
