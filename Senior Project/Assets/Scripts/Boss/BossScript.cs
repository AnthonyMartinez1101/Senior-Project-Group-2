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

    private bool inPlayerRange = false;
    private float hitCooldown = 1.5f;

    public bool phaseTwoActivated = false;

    private void Start()
    {
        currentHealth = data.maxHealth;
        actions = data.actions;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        playerKnockback = player.GetComponent<Knockback>();

        if (!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.SetMax();
    }

    private void Update()
    {
        hitCooldown -= Time.deltaTime;

        if (state == State.Idle && !isPerformingAction)
        {
            QueueAttack();
        }
        if (currentHealth <= data.maxHealth * data.phaseTwoThreshold)
        {
            PhaseTwo();
        }

        if (inPlayerRange && hitCooldown <= 0f)
        {
            playerHealth.TakeDamage(data.attackRating);
            playerKnockback.ApplyKnockback(transform, data.knockbackForce);
            hitCooldown = data.hitCooldown;
        }
    }

    private void QueueAttack()
    {
        isPerformingAction = true;
        BossAction action = actions[Random.Range(0, actions.Count)];
        StartCoroutine(PerformAction(action));
    }

    private IEnumerator PerformAction(BossAction action)
    {
        yield return new WaitForSeconds(data.idleTime);
        state = State.Attacking;
        action.ExecuteAction(this);
        yield return new WaitForSeconds(action.actionDuration);
        state = State.Cooldown;
        yield return new WaitForSeconds(data.cooldownTime);
        state = State.Idle;
        isPerformingAction = false;
    }

    private void PhaseTwo()
    {
        // Increase speed, attack, etc.
        data.attackRating *= data.rageMultiplier;
        data.cooldownTime /= data.rageMultiplier;
        data.idleTime /= data.rageMultiplier;
        phaseTwoActivated = true;
    }

    public void TakeDamage(float damageAmount)
    {
        if (damageFlash) damageFlash.FlashOnDamage();
        currentHealth -= damageAmount;
        Debug.Log("Boss Health: " + currentHealth);
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
