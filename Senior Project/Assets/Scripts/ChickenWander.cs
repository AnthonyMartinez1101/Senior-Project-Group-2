using UnityEngine;
using UnityEngine.AI;

public class ChickenWander : MonoBehaviour
{
    public Transform hidePosition;

    NavMeshAgent agent;

    public WorldClock worldClock;

    public float wanderRadius = 5f;

    private float waitTime = 0f;
    private bool atDestination = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (atDestination)
        {
            waitTime -= Time.deltaTime;

            if (waitTime <= 0f)
            {
                if (TryGetRandomPointOnNavMesh(transform.position, wanderRadius, agent.areaMask, out Vector3 point))
                {
                    agent.SetDestination(point);
                    waitTime = Random.Range(2f, 5f);
                    atDestination = false;
                }
            }
        }
        else
        {
        }

    }

    //*Made with help from ChatGPT*
    static bool TryGetRandomPointOnNavMesh(Vector3 center, float radius, int areaMask, out Vector3 result)
    {
        // Try a few times to find a point
        for (int i = 0; i < 12; i++)
        {
            Vector2 random2D = Random.insideUnitCircle * radius;
            Vector3 candidate = center + new Vector3(random2D.x, random2D.y, 0f);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.5f, areaMask))
            {
                result = hit.position;
                return true;
            }
        }

        result = center;
        return false;
    }

    public void SetHidePos(Transform pos)
    {
        hidePosition = pos;
    }

    public void SetWorldTime(WorldClock wt)
    {
        worldClock = wt;
    }
}
