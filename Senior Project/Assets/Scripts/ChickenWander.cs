using System.Collections;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.AI;

public class ChickenWander : MonoBehaviour
{
    public Transform hidePosition;

    NavMeshAgent agent;

    public WorldClock worldClock;

    public float minWanderRadius = 5f;
    public float maxWanderRadius = 10f;

    public float wanderSpeed = 2f;

    public float minWaitTime = 1f;
    public float maxWaitTime = 5f;

    public SpriteRenderer chickenSprite;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        if(hidePosition == null) hidePosition = GameManager.Instance.GetChickenHidePosition();
        if(worldClock == null) worldClock = GameManager.Instance.GetWorldClock();

        StartCoroutine(Wander());
    }

    IEnumerator Wander()
    {
        while (true)
        {
            //Night hide
            if(worldClock != null && worldClock.IsNight())
            {
                agent.ResetPath();

                if(hidePosition != null) agent.SetDestination(hidePosition.position);
                agent.speed = wanderSpeed * 4f;

                yield return new WaitUntil(() => !agent.pathPending && (!hidePosition || agent.remainingDistance <= 2.5f || agent.pathStatus != NavMeshPathStatus.PathComplete));

                chickenSprite.enabled = false;
                yield return new WaitUntil(() => worldClock.IsDay());
                chickenSprite.enabled = true;

                yield return null;
                continue;
            }

            //Day wander
            if(TryGetRandomPoint(transform.position, minWanderRadius, maxWanderRadius, agent.areaMask, out var dest))
            {
                agent.SetDestination(dest);
                agent.speed = wanderSpeed;
            }
            yield return new WaitUntil(() => worldClock.IsNight() || (!agent.pathPending && (agent.remainingDistance <= 0.25f || agent.pathStatus != NavMeshPathStatus.PathComplete)));
            yield return WaitWhileDay(Random.Range(minWaitTime, maxWaitTime));
        }
    }

    IEnumerator WaitWhileDay(float waitTime)
    {
        float time = 0f;
        while(time < waitTime && worldClock.IsDay())
        {
            time += Time.deltaTime;
            yield return null;
        }
    }

    //*Made with help from ChatGPT*
    static bool TryGetRandomPoint(Vector3 center, float min, float max, int areaMask, out Vector3 result)
    {
        // Try a few times to find a point
        for (int i = 0; i < 50; i++)
        {
            Vector2 randSpot = Random.insideUnitCircle * Random.Range(min, max);
            Vector3 candidate = center + new Vector3(randSpot.x, randSpot.y, 0f);

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
