using UnityEngine;
using UnityEngine.AI;

public class chickenWalk : MonoBehaviour
{
    public Transform A;
    public Transform B;

    private NavMeshAgent agent;
    private Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = A;
        agent.SetDestination(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            target = target == A ? B : A;
            agent.SetDestination(target.position);
        }
    }
}
