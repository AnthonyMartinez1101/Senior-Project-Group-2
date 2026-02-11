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
    public float lungeOvershoot = 1.5f;

    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(ShortJump(boss));  
    }

    private IEnumerator ShortJump(BossScript boss)
    {
        if (boss.phaseTwoActivated)
        {
        yield return boss.StartCoroutine(Forward(boss));
        yield return boss.StartCoroutine(Forward(boss));
        yield return boss.StartCoroutine(Forward(boss));
        yield return boss.StartCoroutine(Forward(boss));
        }
        else
        {
        yield return boss.StartCoroutine(Back(boss));
        yield return boss.StartCoroutine(Forward(boss));    
        }
 
    }

    private IEnumerator Back(BossScript boss)
    {
        Debug.Log("Jumping back!");
        Vector2 player, direction, backTarget, startPosition;
        startPosition = boss.transform.position;

        boss.agent.speed = backSpeed;
        float backTime = 1.5f;
        float elapsed = 0f;
        while (elapsed < backTime)
        {
            player = boss.GetPlayerPosition();
            direction = (startPosition - player).normalized;
            backTarget = startPosition + direction * backDistance;
            boss.agent.SetDestination(backTarget);
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.agent.ResetPath();
    }

    private IEnumerator Forward(BossScript boss)
    {
        Debug.Log("Lunging forward!");
        Vector2 playerPos = boss.GetPlayerPosition();
        Vector2 direction = (playerPos - (Vector2)boss.transform.position).normalized;
        Vector2 lungeTarget = playerPos + direction * lungeOvershoot;

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
