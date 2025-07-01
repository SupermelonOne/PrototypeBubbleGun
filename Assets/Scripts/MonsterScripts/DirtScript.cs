using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirtScript : MonoBehaviour
{
    [SerializeField] private bool requireSoap = true;
    private bool CompletedSoap = false;
    [SerializeField] private bool requireScrub = true;
    private bool CompletedScrub = false;
    [SerializeField] private bool requireWater = false;
    private bool CompletedWater = false;

    [SerializeField] private ParticleSystem bubbleParticles;
    [SerializeField] private ParticleSystem dirtParticles;
    [SerializeField] private ParticleSystem completeParticles;

    


    private float soapValue = 0;
    private float scrubValue = 0;
    private float waterValue = 0;

    private bool visible;
    [SerializeField] private GrabableBone hiddenUnder;
    [SerializeField] private float requiredAngle = 100;
    bool canClean = false;
    [SerializeField] private float maxHealth = 1; //time needs to be cleaned
    private float health;
    //ParticleSystem dirtParticles;
    MonsterCleanness monsterCleanness;

    [SerializeField] private Transform dirtVisual;
    private void Start()
    {
        if (hiddenUnder == null)
        {
            visible = true;
        }
        health = maxHealth;
        monsterCleanness = GetComponentInParent<MonsterCleanness>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Soap"))
        {
            if (soapValue < 1)
            {
                soapValue += Time.deltaTime;
            }
            else
            {
                soapValue = 1;
            }
        }
        if (other.CompareTag("WaterSpray"))
        {
            if (soapValue > 0 && (!requireSoap || CompletedSoap))
            {
                soapValue -= Time.deltaTime;
                if (soapValue <= 0)
                {
                    if (requireScrub)
                    {
                        if (CompletedScrub)
                        {
                            ShowFinish();
                            CompletedWater = true;
                        }
                    }
                    else
                    {
                        CompletedWater = true;
                        ShowFinish();
                    }
                }

            }
            else
            {
                soapValue = 0;
                if (!requireSoap && !requireScrub|| scrubValue >= 1 || !requireScrub && requireSoap && soapValue > 0)
                {
                    waterValue += Time.deltaTime;
                    if (waterValue < 1)
                    {
                        dirtParticles.Play();
                    }
                    else
                    {
                        waterValue = 1;
                        if (!CompletedWater)
                        {
                            CompletedWater = true;
                            ShowFinish();
                        }
                    }
                }
            }

        }
        if (other.CompareTag("Cleaner"))
        {
            if (!requireSoap || soapValue > 0)
            {
                if (scrubValue < 1)
                {
                    scrubValue += Time.deltaTime;
                    if (scrubValue > 1)
                    {
                        scrubValue = 1;
                        if (!CompletedScrub)
                        {
                            CompletedScrub = true;
                            ShowFinish();
                        }
                    }
                    else
                    {
                        dirtParticles.Play();
                    }
                }
            }
            else
            {
                scrubValue += Time.deltaTime;
            }
        }


    }
    private void ShowFinish()
    {
        completeParticles.Play();
    }
    private void Update()
    {
        float soapSize;
        if (soapValue > 0)
        {
            soapSize = 0.5f + soapValue;
        }
        else
        {
            soapSize = 0;
        }

        //Require soap, scrub, water

        if (requireScrub && requireSoap && requireWater)
            SetDirtSize(Mathf.Abs(1 - waterValue));
        else if (requireSoap && requireWater)
            SetDirtSize(Mathf.Abs(1 - waterValue));
        else if (requireSoap)
            Debug.Log("impossible dirt detected");


        bubbleParticles.transform.localScale = new Vector3(soapValue, soapValue, soapValue);
    }

    private void SetDirtSize(float value)
    {

    }

    private float recalculateAngle(float input)
    {
        if (input > 180)
        {
            input -= 360;
        }
        if (input < -180)
        {
            input += 360;
        }
        return input;
    }
    public void GetSoaped()
    {
        canClean = true;
    }
    public void GetDeSoaped()
    {
        canClean = false;
    }
}