using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DirtScript : MonoBehaviour
{
    [SerializeField] private bool requireSoap = true;
    [SerializeField] private bool requireScrub = true;
    [SerializeField] private bool requireWater = false;

    [SerializeField] private ParticleSystem bubbleParticles;
    [SerializeField] private ParticleSystem dirtParticles;

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
/*        if (canClean)
        {
            if (other.CompareTag("Cleaner") && visible)
            {
                health -= Time.deltaTime;
                if (dirtVisual != null)
                {
                    float modelSize = ((health / maxHealth) * 0.7f) + 0.3f;
                    dirtVisual.localScale = new Vector3(modelSize, modelSize, modelSize);
                }
                if (dirtParticles != null)
                {
                    dirtParticles.Play();
                }
            }
        }*/
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
            if (soapValue > 0)
            {
                soapValue -= Time.deltaTime;
            }
            if (waterValue < 1)
            {
                waterValue += Time.deltaTime;
            }
            else
            {
                waterValue = 1;
            }
        }
        if (other.CompareTag("Cleaner"))
        {
            if (requireSoap)
            {
                if (soapValue > 0)
                {
                    scrubValue += Time.deltaTime;
                }
            }
            else
            {
                scrubValue += Time.deltaTime;
            }
        }


    }
    private void Update()
    {
/*        if (health <= 0)
        {
            Destroy(gameObject);
            if (monsterCleanness != null)
            {
                monsterCleanness.RemoveDirt(this);
                monsterCleanness.CheckDirt();
            }
        }
        if (hiddenUnder == null) return;
        Vector3 limbDirection = hiddenUnder.transform.up;

        float angleFromUp = Vector3.Angle(limbDirection, Vector3.up);

        bool visible = angleFromUp <= requiredAngle;*/



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