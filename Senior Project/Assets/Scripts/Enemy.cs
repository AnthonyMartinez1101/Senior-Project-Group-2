using UnityEngine;
using WorldTime;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 5f;
    [SerializeField] float dayBurnTime = 5f;
    private FloatingHealth healthBar;
    [SerializeField] private float dealDamage;
    [SerializeField] private float knockbackForce;

    private WorldTime.WorldTime worldTime;

    private bool inPlayerRange;

    private PlayerHealth playerHealth;
    private Knockback playerKnockback;

    [SerializeField] Item randomItemDrop;

    private DamageFlash damageFlash;

    private float hitCooldown = 1.5f;



    //Damage per second taken during the day
    private float dps;


    private void Start()
    {
        health = maxHealth;

        if(!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.SetMax();

        inPlayerRange = false;

        EnemyFollow enemyFollow = GetComponent<EnemyFollow>();
        Transform player = enemyFollow.GetTarget();

        if(player)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerKnockback = player.GetComponent<Knockback>();
        }

        dps = maxHealth / dayBurnTime;

        damageFlash = GetComponent<DamageFlash>();
    }


    public void TakeDamage(float damageAmount)
    {
        if(damageFlash) damageFlash.FlashOnDamage();
        health -= damageAmount;
        if(healthBar) healthBar.UpdateHealth(health, maxHealth);
        if (health <= 0)
        {
            int randNum = Random.Range(0, 100);
            if(randNum == 1 && randomItemDrop != null) ItemDropFactory.Instance.SpawnItem(randomItemDrop, transform.position);
            Destroy(gameObject);
        }
    }

    private void DayDamage(float damageAmount)
    {
        health -= damageAmount;
        if (healthBar) healthBar.UpdateHealth(health, maxHealth);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        hitCooldown -= Time.deltaTime;

        //If day, take damage
        if (worldTime.CurrentPhase == Phase.Day)
        {
            DayDamage(dps * Time.deltaTime);
        }

        //While colliding with player, deal constant damage over time
        if (inPlayerRange && hitCooldown < 0f)
        {
            playerHealth.TakeDamage(dealDamage);
            playerKnockback.ApplyKnockback(transform, knockbackForce);
            hitCooldown = 1.5f;
        }
    }

    //When colliding with something...
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //...if colliding with player, deal constant damage via bool 
        if (collision.CompareTag("Player"))
        {
            inPlayerRange = true;
        }

        //...if colliding with plant, deal damage to plant once upon touching
        if (collision.CompareTag("Plant"))
        {
            PlantScript plant = collision.GetComponent<PlantScript>();
            plant.TakeDamage(1f);
        }
    }

    //When no longer colliding with something...
    private void OnTriggerExit2D(Collider2D collision)
    {
        //..if no longer colliding with player, stop dealing damage via bool 
        if (collision.CompareTag("Player"))
        {
            inPlayerRange = false;
        }
    }

    public void SetWorldTime(WorldTime.WorldTime wt)
    {
        worldTime = wt;
    }
}
