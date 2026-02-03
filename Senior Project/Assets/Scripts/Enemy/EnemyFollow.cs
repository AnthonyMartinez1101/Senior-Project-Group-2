using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    private Transform target;
    NavMeshAgent agent;

    private Knockback kb;
    private float currentSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        kb = GetComponent<Knockback>();
    }


    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if(kb.IsKnockbackActive())
            {
                currentSpeed = 1f;
            }
            agent.SetDestination(target.position);
        }
        agent.speed = currentSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, 3.5f, Time.deltaTime * 1.5f);
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public Transform GetTarget()
    {
        return target;
    }
}
 