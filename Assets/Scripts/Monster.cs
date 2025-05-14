using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(MonsterMoveBehavior))]
public class Monster : MonoBehaviour
{
    private MonsterMoveBehavior moveBehavior;
    [HideInInspector] public bool isCaptured = false;

    [SerializeField] private GameObject bubble;
    private void Start()
    {
        //in case we want multiple move behaviours
        moveBehavior = GetComponent<MonsterMoveBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bubble"))
        {
            CaptureMonster();
            StartCoroutine(CountDownAndRelease(5.0f));
        }
    }

    private void CaptureMonster()
    {
        isCaptured = true;
        bubble.SetActive(true);
        moveBehavior.StopMoving(5.0f);
    }

    private void ReleaseMonster()
    {
        isCaptured = false;
        bubble.SetActive(false);
        //moveBehavior.canMove = true;
    }

    public void PutInNet(Transform netTransform)
    {
        moveBehavior.Capture(netTransform);
    }

    private IEnumerator CountDownAndRelease(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        bubble.SetActive(false);
        ReleaseMonster();
    }
}
