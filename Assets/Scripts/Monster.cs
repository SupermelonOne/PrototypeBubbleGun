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
    private ParticleSystem caughtParticles;
    private AudioSource catchSound;
    [SerializeField] private List<AudioClip> catchSounds = new List<AudioClip>();

    [SerializeField] private GameObject bubble;
    private void Start()
    {
        //in case we want multiple move behaviours
        moveBehavior = GetComponent<MonsterMoveBehavior>();
        if (caughtParticles == null)
            caughtParticles = GetComponentInChildren<ParticleSystem>();
        if (catchSound == null)
        {
            catchSound = GetComponent<AudioSource>();
            if (catchSounds.Count > 0)
            {
                catchSound.clip = catchSounds[UnityEngine.Random.Range(0, catchSounds.Count)];
            }
        }
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
        if (caughtParticles != null)
        {
            caughtParticles.Play();
        }
        if (catchSound != null)
        {
            catchSound.pitch = UnityEngine.Random.Range(0.85f, 1.2f);
            catchSound.Play();
        }
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
