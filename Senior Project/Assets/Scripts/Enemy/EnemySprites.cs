using UnityEngine;
using UnityEngine.AI;

public class EnemySprites : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(agent == null || animator == null) return;

        Vector2 moveDir = new Vector2(agent.velocity.x, agent.velocity.y);
        if(moveDir.magnitude <= 0.1f) return;

        Vector2 normalised = moveDir.normalized;
        animator.SetFloat("moveX", normalised.x);
        animator.SetFloat("moveY", normalised.y);
    }
}
