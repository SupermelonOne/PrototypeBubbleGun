using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRandom : MonoBehaviour
{
    private Transform trans;
    private void Start()
    {
        trans = GetComponent<Transform>();
    }

    public void DoAction()
    {
        trans.position = new Vector3(100, 100, 100);
    }
}
