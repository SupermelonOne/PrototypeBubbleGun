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
    MonsterCleanness monsterCleanness;

    private MonsterMoveBehavior moveBehavior;
    [HideInInspector] public bool isCaptured = false;
    [SerializeField] private ParticleSystem caughtParticles;
    [SerializeField] private ParticleSystem sprayParticles;
    private AudioSource catchSound;
    [SerializeField] private List<AudioClip> catchSounds = new List<AudioClip>();

    private float soapiness = 0;

    [SerializeField] private GameObject bubble;
    private void Start()
    {
        monsterCleanness = GetComponent<MonsterCleanness>();
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
            //CaptureMonster();
            //StartCoroutine(CountDownAndRelease(5.0f));
            SoapMonster();
            bubble.transform.localScale = new Vector3(soapiness / 2, soapiness / 2, soapiness / 2);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("WaterSpray"))
        {
            if (soapiness > 0)
            {
                soapiness -= Time.deltaTime;
                if (sprayParticles != null)
                    sprayParticles.Play();
                if (soapiness <= 0)
                {
                    monsterCleanness.DeSoaped();
                }
                bubble.transform.localScale = new Vector3(soapiness / 2, soapiness/2, soapiness/2);
            }
        }
    }
    private void SoapMonster()
    {
        if (caughtParticles != null)
        {
            Debug.Log("shouldve player");

            caughtParticles.Play();
        }
        if (catchSound != null)
        {
            catchSound.pitch = UnityEngine.Random.Range(0.85f, 1.2f);
            catchSound.Play();
        }
        bubble.SetActive(true);
        soapiness = 2;
        if (monsterCleanness != null)
        {
            monsterCleanness.GetSoaped();
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
