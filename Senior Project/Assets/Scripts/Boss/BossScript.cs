using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;


public class BossScript : MonoBehaviour
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
    private PlayerHealth playerHealth;
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

    private void Start()
    {
        currentHealth = data.maxHealth;
        actions = data.actions;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindGameObjectWithTag("Player").transform;
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

    public void ApplyPoison(int ticks)
    {
        poisonCount = ticks;
        StartCoroutine(PoisonDamage());
    }

    IEnumerator PoisonDamage()
    {
        while (poisonCount > 0)
        {
            TakeDamage(1f);
            poisonCount--;
            yield return new WaitForSeconds(0.75f);
        }
    }

    private void QueueAttack()
    {
        isPerformingAction = true;
        BossAction action = actions[Random.Range(0, actions.Count)];
        Debug.Log("Boss is performing: " + action.name);
        StartCoroutine(PerformAction(action));
    }

    public Vector2 GetPlayerPosition()
    {
        if(player != null) return player.position;
        else return Vector2.zero;
    }

    private IEnumerator PerformAction(BossAction action)
    {
        yield return new WaitForSeconds(idleAmount);
        state = State.Attacking;
        action.ExecuteAction(this);
        yield return new WaitForSeconds(action.actionDuration);
        state = State.Cooldown;
        yield return new WaitForSeconds(cooldownAmount);
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

    public void TakeDamage(float damageAmount)
    {
        // If shield is active, ignore/don't apply damage to boss and log for verification
        if (isShielded | data.isInvincible)
        {
            return;
        }

        if (damageFlash) damageFlash.FlashOnDamage();
        currentHealth -= damageAmount;
        //Debug.Log("Boss Health: " + currentHealth);
        if (healthBar) healthBar.UpdateHealth(currentHealth, data.maxHealth);
        if (currentHealth <= 0)
        {
            //int randNum = Random.Range(0, 100);
            //if (randNum <= dropChance && randomItemDrop != null) ItemDropFactory.Instance.SpawnItem(randomItemDrop, 0, transform.position, dropExpires);
            Destroy(gameObject);
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
}
