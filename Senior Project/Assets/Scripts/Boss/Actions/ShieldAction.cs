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
            Debug.Log($"[Shield] Boss is now shielded.");

            if (owner.agent != null && speedMultiplier > 0f && !Mathf.Approximately(speedMultiplier, 1f))
            {
                originalAgentSpeed = owner.agent.speed;

                speedOverride = owner.GetComponent<AgentSpeedOverride>();
                if (speedOverride == null)
                {
                    speedOverride = owner.gameObject.AddComponent<AgentSpeedOverride>();
                }

                speedOverride.Initialize(owner.agent, originalAgentSpeed, speedMultiplier);
                speedModified = true;

                Debug.Log($"[Shield] Shield is making boss slow. New speed is {originalAgentSpeed * speedMultiplier:F2}");
            }
        }
        else
        {
            Debug.Log("[Shield] Initialized with no boss.");
        }
    }

    public void TakeDamage(float dmg = 1f)
    {
        curHealth -= dmg;
        Debug.Log($"[Shield] Took {dmg} damage. Remaining HP: {curHealth}");

        if (curHealth <= 0f)
        {
            Debug.Log($"[Shield] Shield destroyed on boss '{(owner != null ? owner.name : "unknown")}'.");
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (owner != null)
        {
            owner.isShielded = false;

            if (speedModified && owner.agent != null)
            {
                if (speedOverride != null)
                {
                    speedOverride.RestoreAndRemove();
                    speedOverride = null;
                }
                else
                {
                    owner.agent.speed = originalAgentSpeed;
                }

                speedModified = false;
                Debug.Log($"[Shield] Boss speed returned to normal: {owner.agent.speed:F2}.");
            }

            Debug.Log($"[Shield] OnDestroy: cleared the shield.");
        }
        else
        {
            Debug.Log("[Shield] OnDestroy: No boss.");
        }
    }
}