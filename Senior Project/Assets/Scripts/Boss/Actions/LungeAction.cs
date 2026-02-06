using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Lunge", menuName = "Boss/Actions/Lunge")]
public class Lunge : BossAction
{
    public float lungeSpeed = 10f;
    public float backSpeed = 5f;
    public float backDistance = 5f;
    public float ptwoRage = 1.5f;
    public float backDivider = 1.5f;

    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(ShortJump(boss));  
    }

    private IEnumerator ShortJump(BossScript boss)
    {
        yield return boss.StartCoroutine(Back(boss));
        yield return boss.StartCoroutine(Forward(boss));
    }

    private IEnumerator Back(BossScript boss)
    {
        Debug.Log("Jumping back!");
        Vector2 startPosition = boss.transform.position;
        Vector2 player = boss.player.position;
        Vector2 direction = (startPosition - player).normalized;
        float backMod = backDistance;
        if (boss.phaseTwoActivated)
        {
            backMod = backDistance / Mathf.Max(0.0001f, ptwoRage);
        }
        Vector2 backTarget = startPosition + direction * backMod;

        boss.agent.speed = backSpeed;
        boss.agent.SetDestination(backTarget);

        float backTime = 2f;
        float elapsed = 0f;
        while (Vector2.Distance((Vector2)boss.transform.position, backTarget) > 0.05f && elapsed < backTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.agent.ResetPath();
    }

    private IEnumerator Forward(BossScript boss)
    {
        Debug.Log("Lunging forward!");
        Vector2 lungeTarget = boss.player.position;

        float ogAcceleration = boss.agent.acceleration;
        boss.agent.acceleration = 100f;

        if (boss.phaseTwoActivated)
        {
            boss.agent.acceleration = 120f;
            boss.agent.speed = (lungeSpeed * ptwoRage);
        }
        else boss.agent.speed = lungeSpeed;
        boss.agent.SetDestination(lungeTarget);

        float time = 0f;
        while (time < actionDuration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        boss.agent.ResetPath();
        boss.agent.acceleration = ogAcceleration;
    }
}
