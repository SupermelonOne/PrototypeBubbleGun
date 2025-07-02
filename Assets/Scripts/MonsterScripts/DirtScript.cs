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

    private bool Cleaned = false;

    [SerializeField] private ParticleSystem bubbleParticles;
    [SerializeField] private ParticleSystem dirtParticles;
    [SerializeField] private ParticleSystem completeParticles;


    [SerializeField] private float requiredWaterValue = 4;
    [SerializeField] private float requiredScrubValue = 1.5f;

    private float soapValue = 0;
    private float scrubValue = 0;
    private float waterValue = 0;

    private bool visible;
    [SerializeField] private GrabableBone hiddenUnder;
    [SerializeField] private float requiredAngle = 100;
    bool canClean = false;
    private float maxHealth = 1; //time needs to be cleaned
    private float health;
    //ParticleSystem dirtParticles;

    [SerializeField] private AudioSource completeSound;
    [SerializeField] private AudioSource dirtSound;



    //A NULL REFERENCE TO THE MONSTERCLEANNESS COULD BE THAT THE REFERENCE IS NEVER PROPERLY SET DUE TO THE GetComponentInParent<MonsterCleanness>() IN START
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

        if (monsterCleanness == null)
            return;
        monsterCleanness.AddDirt(this);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Soap"))
        {
            Debug.Log("found souap");
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
                    if (waterValue < requiredWaterValue)
                    {
                        ShowDirtPart();
                    }
                    else
                    {
                        waterValue = requiredWaterValue;
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
                if (scrubValue < requiredScrubValue)
                {
                    scrubValue += Time.deltaTime;
                    if (scrubValue > requiredScrubValue)
                    {
                        scrubValue = requiredScrubValue;
                        if (!CompletedScrub)
                        {
                            CompletedScrub = true;
                            ShowFinish();
                        }
                    }
                    else
                    {
                        ShowDirtPart();
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
        completeSound.Play();
    }
    private void ShowDirtPart()
    {
        dirtParticles.Play();

        if (dirtSound == null || Random.Range(0, 30) > 2)
            return;
        dirtSound.pitch = Random.Range(0.8f, 1.2f);
        dirtSound.Play();
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

        float dirtSize = Mathf.Abs(1 - (waterValue / requiredWaterValue));

        if (requireScrub && requireSoap && requireWater)
            SetDirtSize(dirtSize);
        else if (requireSoap && requireWater)
            SetDirtSize(dirtSize);
        else if (requireScrub && requireWater)
            SetDirtSize(dirtSize);
        if (dirtSize <= 0 && !Cleaned)
        {
            Debug.Log("cleaned this piece of dirt");
            Cleaned = true;
            monsterCleanness.RemoveDirt(this);
        }

        bubbleParticles.transform.localScale = new Vector3(soapValue, soapValue, soapValue);
    }

    private void SetDirtSize(float value)
    {
        dirtVisual.localScale = new Vector3(value, value, value);   
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