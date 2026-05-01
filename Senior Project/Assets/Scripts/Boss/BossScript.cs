using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;


public class BossScript : MonoBehaviour, IDamageable, IPoisonable
{
    public enum State
    {
        Idle,
        Attacking,
        Cooldown
    }

    public BossData data;
    public SpriteRenderer spriteRenderer;

    private float currentHealth;

    private State state = State.Idle;
    private List<BossAction> actions;
    private bool isPerformingAction = false;

    public Transform player;
    public NavMeshAgent agent;
    public PlayerHealth playerHealth;
    private Knockback playerKnockback;
    public DamageFlash damageFlash;
    private FloatingHealth healthBar;

    private float attackValue, cooldownAmount, idleAmount;

    private bool inPlayerRange = false;
    private float hitCooldown = 1.5f;

    public bool phaseTwoActivated = false;

    // Flags for a shield state added.
    public bool isShielded = false;
    public bool hasUsedShield = false;

    private int poisonCount = 0;

    private int defence = 1;

    public GameObject deathParticles;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = data.maxHealth;
        actions = data.actions;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if(!player) player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        playerKnockback = player.GetComponent<Knockback>();

        if (!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.SetMax();

        attackValue = data.attackRating;
        cooldownAmount = data.cooldownTime;
        idleAmount = data.idleTime;

        if (data.bossSprite != null) spriteRenderer.sprite = data.bossSprite;
    }

    private void Update()
    {
        if (isDead) return;

        if (!player)
        {
            StopAllCoroutines();
            return;
        }

        hitCooldown -= Time.deltaTime;

        if (state == State.Idle && !isPerformingAction)
        {
            QueueAttack();
        }
        if (currentHealth <= data.maxHealth * data.phaseTwoThreshold && !phaseTwoActivated)
        {
            PhaseTwo();
        }

        if (inPlayerRange && hitCooldown <= 0f)
        {
            playerHealth.TakeDamage(attackValue);
            playerKnockback.ApplyKnockback(transform, data.knockbackForce);
            hitCooldown = data.hitCooldown;
        }
    }

    public void ApplyDifficulty(int difficulty)
    {
        defence *= difficulty;
    }

    public void ApplyPoison(int ticks)
    {
        bool ifPoisoned = poisonCount > 0;

        poisonCount = ticks;

        if (!ifPoisoned) StartCoroutine(PoisonDamage());
    }

    IEnumerator PoisonDamage()
    {
        while (poisonCount > 0)
        {
            if (isDead) yield break;
            TakeDamage(1f, DamageType.Poison);
            poisonCount--;
            yield return new WaitForSeconds(0.75f);
        }
    }

    private void QueueAttack()
    {
        if (isDead) return;

        isPerformingAction = true;
        BossAction action;
        int safetyCount = 0;
        while (safetyCount < 1000)
        {
            if (isDead) return;
            action = actions[Random.Range(0, actions.Count)];
            Debug.Log("Boss is performing: " + action.name);
            if (Random.value <= action.activationChance)
            {
                StartCoroutine(PerformAction(action));
                return;
            }
            safetyCount++;
        }
    }

    public Vector2 GetPlayerPosition()
    {
        if(player != null) return player.position;
        else return Vector2.zero;
    }

    private IEnumerator PerformAction(BossAction action)
    {
        if (isDead) yield break;
        yield return new WaitForSeconds(idleAmount);
        if (isDead) yield break;
        state = State.Attacking;
        action.ExecuteAction(this);
        yield return new WaitForSeconds(action.actionDuration);
        if (isDead) yield break;
        state = State.Cooldown;
        yield return new WaitForSeconds(cooldownAmount);
        if (isDead) yield break;
        state = State.Idle;
        isPerformingAction = false;
    }

    private void PhaseTwo()
    {
        attackValue *= data.rageMultiplier;
        cooldownAmount /= data.rageMultiplier;
        idleAmount /= data.rageMultiplier;
        phaseTwoActivated = true;
        if (data.altSprite != null) spriteRenderer.sprite = data.altSprite;
    }

    public void TakeDamage(float damageAmount, DamageType damageType)
    {
        //Return, already dead
        if (isDead) return;
        if (currentHealth <= 0) return;
        if(damageAmount <= 0) return;

        // If shield is active, ignore/don't apply damage to boss and log for verification
        if (isShielded || data.isInvincible)
        {
            return;
        }

        if (damageFlash) damageFlash.FlashOnDamage();
        currentHealth -= (damageAmount / defence);
        //Debug.Log("Boss Health: " + currentHealth);
        if (healthBar) healthBar.UpdateHealth(currentHealth, data.maxHealth);
        if (currentHealth <= 0)
        {
            StopAllCoroutines();
            Die();
        }
    }

    private void Die()
    {
        if(isDead) return;
        isDead = true;
        if (data.itemDrop) Instantiate(data.itemDrop, transform.position, Quaternion.identity);

        if (data.coin)
        {
            for(int i = 0; i < 10; i++)
            {
                ItemDropFactory.Instance.SpawnItem(data.coin, 0, transform.position, false); 
            }
        }

        StatManager.Instance.AddBossKill();
        if (deathParticles) Instantiate(deathParticles, transform.position, transform.rotation);
        Destroy(gameObject);

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
            plant.TakeDamage(1f, DamageType.Enemy);
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
}
