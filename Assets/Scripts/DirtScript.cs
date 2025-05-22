using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtScript : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1; //time needs to be cleaned
    private float health;
    ParticleSystem particleSystem;
    MonsterCleanness monsterCleanness;

    [SerializeField] private Transform dirtVisual;
    private void Start()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        health = maxHealth;
        monsterCleanness = GetComponentInParent<MonsterCleanness>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cleaner"))
        {
            health -= Time.deltaTime;
            if (dirtVisual != null)
            {
                float modelSize = ((health / maxHealth) * 0.7f)+ 0.3f;
                dirtVisual.localScale = new Vector3(modelSize, modelSize, modelSize);
            }
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }
    }
    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            if (monsterCleanness != null)
            {
                monsterCleanness.RemoveDirt(this);
                monsterCleanness.CheckDirt();
            }
        }
    }
}
