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
        //debugging is my passion
        Debug.Log("arrrr, ive been shot in me penis");
        CaptureMonster();
        StartCoroutine(CountDownAndRelease(5.0f));
    }

    private void CaptureMonster()
    {
        isCaptured = true;
        bubble.SetActive(true);
        moveBehavior.canMove = false;
    }

    private void ReleaseMonster()
    {
        isCaptured = false;
        bubble.SetActive(false);
        moveBehavior.canMove = true;
    }

    private IEnumerator CountDownAndRelease(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ReleaseMonster();
    }
}
