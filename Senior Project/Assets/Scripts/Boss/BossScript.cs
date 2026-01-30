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

    private void Start()
    {
        currentHealth = data.maxHealth;
        actions = data.actions;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        float hitCooldown = data.hitCooldown;
        hitCooldown -= Time.deltaTime;

        if (state == State.Idle && !isPerformingAction)
        {
            QueueAttack();
        }
        if (currentHealth <= data.maxHealth * data.phaseTwoThreshold)
        {
            PhaseTwo();
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

    }
}
