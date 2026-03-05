using UnityEngine;

public class AgentSpeedOverride : MonoBehaviour
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
        enabled = true;
    }

    void LateUpdate()
    {
        if (!active || agent == null) return;
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