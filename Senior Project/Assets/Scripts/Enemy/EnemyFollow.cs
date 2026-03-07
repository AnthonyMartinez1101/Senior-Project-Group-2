using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    private Transform target;
    NavMeshAgent agent;

    private Knockback kb;
    private float currentSpeed;
    public float startingTopSpeed = 3.5f;
    public float maxTopSpeed = 5f;

    [Range(0, 100)]
    public int speedIncreasePercentage = 5;
    public float timeToIncrease = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        kb = GetComponent<Knockback>();

        StartCoroutine(IncreaseSpeed());
    }


    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            target = GameManager.Instance.player.transform;
            return;
        }

        if(kb.IsKnockbackActive())
        {
            currentSpeed = 1f;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > 25)
        {
            agent.speed = 10f;
        }
        else
        {
            agent.speed = currentSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, startingTopSpeed, Time.deltaTime * 1.5f);
        }
        agent.SetDestination(target.position);
    }

    IEnumerator IncreaseSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToIncrease);
            startingTopSpeed *= 1 + (speedIncreasePercentage / 100f);
            startingTopSpeed = Mathf.Min(startingTopSpeed, maxTopSpeed);
        }
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
 