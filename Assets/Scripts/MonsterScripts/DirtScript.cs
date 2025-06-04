using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtScript : MonoBehaviour
{
    bool canClean = false;
    [SerializeField] private float maxHealth = 1; //time needs to be cleaned
    private float health;
    ParticleSystem particleSystem;
    MonsterCleanness monsterCleanness;

    [SerializeField] private int points = 5;

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }

    [SerializeField] private Transform dirtVisual;
    private void Start()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        health = maxHealth;
        monsterCleanness = GetComponentInParent<MonsterCleanness>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (canClean)
        {
            if (other.CompareTag("Cleaner"))
            {
                health -= Time.deltaTime;
                if (dirtVisual != null)
                {
                    float modelSize = ((health / maxHealth) * 0.7f) + 0.3f;
                    dirtVisual.localScale = new Vector3(modelSize, modelSize, modelSize);
                }
                if (particleSystem != null)
                {
                    particleSystem.Play();
                }
            }
        }
    }
    private void Update()
    {
        if (health <= 0)
        {
            OnClean(points, 1);
            Destroy(gameObject);
            if (monsterCleanness != null)
            {
                monsterCleanness.RemoveDirt(this);
                monsterCleanness.CheckDirt();
            }
        }
    }
    public void GetSoaped()
    {
        canClean = true;
    }
    public void GetDeSoaped()
    {
        canClean = false;
    }

    private void OnClean(int points, float amplifier)
    {
        int pointsGranted = (int)(points * amplifier);
        MonsterEventBus.Invoke(new MonsterEventBus.DirtClean(pointsGranted));
    }
}