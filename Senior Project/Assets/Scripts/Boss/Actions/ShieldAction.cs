using System.Collections;
using UnityEngine;

public class ShieldAction : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float curHealth;

    private BossScript owner;
    private float originalAgentSpeed;
    private bool speedModified = false;
    private AgentSpeedOverride speedOverride;

    void Start()
    {
        if (curHealth <= 0f) curHealth = maxHealth;
    }

    public void Initialize(BossScript boss, float speedMultiplier = 1f)
    {
        owner = boss;
        curHealth = maxHealth;

        if (owner != null)
        {
            owner.isShielded = true;

            // Apply movement speed multiplier
            if (owner.agent != null && speedMultiplier > 0f && !Mathf.Approximately(speedMultiplier, 1f))
            {
                // Save the current speed so we can restore later
                originalAgentSpeed = owner.agent.speed;

                // Add the override component on the boss to force the effective speed in LateUpdate
                speedOverride = owner.GetComponent<AgentSpeedOverride>();
                if (speedOverride == null)
                {
                    speedOverride = owner.gameObject.AddComponent<AgentSpeedOverride>();
                }

                speedOverride.Initialize(owner.agent, originalAgentSpeed, speedMultiplier);
                speedModified = true;
            }
        }
        else
        {
        }
    }

    /// Apply damage to the shield.
    public void TakeDamage(float dmg = 1f)
    {
        curHealth -= dmg;

        if (curHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Clear the shield flag when the shield GameObject is destroyed
        if (owner != null)
        {
            owner.isShielded = false;

            // Remove the override and restore the boss' original movement speed
            if (speedModified && owner.agent != null)
            {
                if (speedOverride != null)
                {
                    speedOverride.RestoreAndRemove();
                    speedOverride = null;
                }
                else
                {
                    // Fallback: restore directly if override component was not found
                    owner.agent.speed = originalAgentSpeed;
                }

                speedModified = false;
            }
        }
        else
        {
        }
    }

    // Beetle specific helper component to enforce speed modifier
    private class AgentSpeedOverride : MonoBehaviour
    {
        private UnityEngine.AI.NavMeshAgent agent;
        private float baseSpeed;
        private float multiplier = 1f;
        private bool active = false;

        public void Initialize(UnityEngine.AI.NavMeshAgent agent, float baseSpeed, float multiplier)
        {
            this.agent = agent;
            this.baseSpeed = baseSpeed;
            this.multiplier = multiplier;
            active = true;
            // Ensure this component runs after other scripts where possible by leaving it as LateUpdate enforcer.
            enabled = true;
        }

        void LateUpdate()
        {
            if (!active || agent == null) return;
            // Force the agent speed each frame in LateUpdate so Update/coroutine assignments are overridden.
            agent.speed = baseSpeed * multiplier;
        }

        public void RestoreAndRemove()
        {
            if (agent != null)
            {
                agent.speed = baseSpeed;
            }
            active = false;
            Destroy(this);
        }
    }
}