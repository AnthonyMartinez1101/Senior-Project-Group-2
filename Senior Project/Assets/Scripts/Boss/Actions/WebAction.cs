using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Web", menuName = "Boss/Actions/Web")]

public class WebAction : BossAction
{
    public GameObject webPrefab;
    public float throwSpeed = 15f;
    public float stopDistance = 0.1f;
    public string animTrigger = "SpawnWeb";

    private float actionTimer = 0f;

    private Animator animator;

    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(DoWebAction(boss));
    }

    IEnumerator DoWebAction(BossScript boss)
    {
        // Reset timer each time the action starts so repeated uses behave correctly
        actionTimer = 0f;

        if (animator == null)
        {
            animator = boss.GetComponent<Animator>(); //if its empty assign it
        }

        //boss.agent.isStopped = true; // Stop the boss from moving while performing the action

        while (actionTimer < actionDuration)
        {
            actionTimer += Time.deltaTime;

            if (TryGetRandomPoint(boss.transform.position, 15f, 30f, NavMesh.AllAreas, out Vector3 targetPoint))
            {
                yield return null;
                continue;
            }
            boss.agent.SetDestination(targetPoint);

            float randTime = Random.Range(3f, 5f);
            float elapsedTime = 0f;

            while (elapsedTime < randTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Instantiate(webPrefab, boss.transform.position, Quaternion.identity);
            if (animator != null)
            {
                animator.SetTrigger(animTrigger);
                Debug.Log("web!");
            }

            Debug.Log($"{actionTimer}, {actionDuration}");
            yield return null;
        }
    }


    static bool TryGetRandomPoint(Vector3 center, float min, float max, int areaMask, out Vector3 result)
    {
        // Try a few times to find a point
        for (int i = 0; i < 50; i++)
        {
            Vector2 randSpot = Random.insideUnitCircle * Random.Range(min, max);
            // Random.insideUnitCircle returns X/Y. In 3D world Y is up, so map to X/Z.
            Vector3 candidate = center + new Vector3(randSpot.x, 0f, randSpot.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 1.5f, areaMask))
            {
                result = hit.position;
                return true;
            }
        }

        result = center;
        return false;
    }
}
