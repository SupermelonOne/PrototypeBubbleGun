using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DirtScript : MonoBehaviour
{
    private bool canClean = false;
    [SerializeField] private float maxHealth = 1; //time needs to be cleaned
    private float health;
    //honestly I don't know why this has to be new but otherwise the compiler yells, so I'll just do what it tells me
    private new ParticleSystem particleSystem;
    private MonsterCleanness monsterCleanness;

    [SerializeField] private Transform dirtVisual;
    private void Start()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
        health = maxHealth;
        monsterCleanness = GetComponentInParent<MonsterCleanness>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (!canClean || !other.CompareTag("Cleaner")) return;
        
        health -= Time.deltaTime;
        if (dirtVisual != null)
        {
            float modelSize = ((health / maxHealth) * 0.7f) + 0.3f;
            dirtVisual.localScale = new Vector3(modelSize, modelSize, modelSize);
        }
        particleSystem.Play();
        
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
    public void GetSoaped()
    {
        canClean = true;
    }
    public void GetDeSoaped()
    {
        canClean = false;
    }
}
