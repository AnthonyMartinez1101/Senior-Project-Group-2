using System.Collections;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "Chase", menuName = "Boss/Actions/Chase")]
public class Chase : BossAction
{
    [Tooltip("Chase speed multiplier (e.g. 1.5 = 50% faster)")]
    public float chaseSpeedMultiplier = 1.6f;

    [Tooltip("Chase damage multiplier (e.g. 1.25 = 25% more damage)")]
    public float damageMultiplier = 1.25f;

    [Tooltip("Chase acceleration multiplier (e.g. 2.0 = 100% faster)")]
    public float chaseAccelerationMultiplier = 2.0f;

    [Tooltip("Dash during chase speed multiplier (e.g. 2.6 = 160% faster)")]
    public float burstSpeedMultiplier = 2.6f;

    [Tooltip("Burst duration")]
    public float burstDuration = 0.18f;

    [Tooltip("Burst cooldown")]
    public float burstInterval = 0.9f;

    [Tooltip("Chase tint color")]
    public Color chaseTint = Color.red;

    public override void ExecuteAction(BossScript boss)
    {
        if (boss == null) return;

        if (!boss.phaseTwoActivated)
        {
            actionDuration = 0f;
            return;
        }

        boss.StartCoroutine(ChaseCoroutine(boss));
    }

    private IEnumerator ChaseCoroutine(BossScript boss)
    {
        if (boss.agent == null)
        {
            //Debug.LogWarning("[Chase] Boss has no NavMeshAgent. Aborting Chase action.");
            yield break;
        }

        float duration = actionDuration;
        if (duration <= 0f) duration = 4f;

        // Save original boss variables to restore after action ends
        UnityEngine.AI.NavMeshAgent agent = boss.agent;
        float originalAgentSpeed = agent.speed;
        float originalAcceleration = agent.acceleration;
        float originalAngularSpeed = agent.angularSpeed;
        Color? originalColor = null;
        if (boss.spriteRenderer != null)
        {
            originalColor = boss.spriteRenderer.color;
            boss.spriteRenderer.color = chaseTint;
        }

        AgentSpeedOverride speedOverride = boss.GetComponent<AgentSpeedOverride>();
        if (speedOverride == null)
        {
            speedOverride = boss.gameObject.AddComponent<AgentSpeedOverride>();
        }
        // Initialize the chase stats for boss
        speedOverride.Initialize(agent, originalAgentSpeed, chaseSpeedMultiplier);
        agent.acceleration = originalAcceleration * chaseAccelerationMultiplier;
        agent.angularSpeed = Mathf.Max(200f, originalAngularSpeed * 1.5f);

        // Patch private attackValue so hits during chase do more damage
        FieldInfo attackField = typeof(BossScript).GetField("attackValue", BindingFlags.Instance | BindingFlags.NonPublic);
        float originalAttackValue = 0f;
        bool attackFieldPatched = false;
        if (attackField != null)
        {
            object v = attackField.GetValue(boss);
            if (v is float f) originalAttackValue = f;
            attackField.SetValue(boss, originalAttackValue * damageMultiplier);
            attackFieldPatched = true;
        }

        float elapsed = 0f;
        float timeSinceLastBurst = burstInterval;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            timeSinceLastBurst += Time.deltaTime;

            agent.SetDestination(boss.GetPlayerPosition());

            // burst logic handling
            if (timeSinceLastBurst >= burstInterval)
            {
                timeSinceLastBurst = 0f;

                // perform a short burst by reinitializing the override with burst multiplier
                speedOverride.Initialize(agent, originalAgentSpeed, burstSpeedMultiplier);

                // keep burst for burstDuration
                float burstTimer = 0f;
                while (burstTimer < burstDuration && elapsed < duration)
                {
                    burstTimer += Time.deltaTime;
                    elapsed += Time.deltaTime;
                    agent.SetDestination(boss.GetPlayerPosition());
                    yield return null;
                }

                // restore base chase speed
                speedOverride.Initialize(agent, originalAgentSpeed, chaseSpeedMultiplier);
            }
            yield return null;
        }

        // Restore attack value, performs check for the stat field
        if (attackFieldPatched)
        {
            attackField.SetValue(boss, originalAttackValue);
        }

        // Restore stats to boss and remove speed modifier
        if (speedOverride != null)
        {
            speedOverride.RestoreAndRemove();
        }
        else
        {
            agent.speed = originalAgentSpeed;
        }

        agent.acceleration = originalAcceleration;
        agent.angularSpeed = originalAngularSpeed;
        agent.ResetPath();

        // return original color
        if (boss.spriteRenderer != null && originalColor.HasValue)
        {
            boss.spriteRenderer.color = originalColor.Value;
        }
    }
}