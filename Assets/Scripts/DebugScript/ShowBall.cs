using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowBall : MonoBehaviour
{
    public float radius = 1;
    public Color sphereColor = Color.red;
    private void OnDrawGizmos()
    {
        Gizmos.color = sphereColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
