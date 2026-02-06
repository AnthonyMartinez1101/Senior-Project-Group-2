using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Lunge", menuName = "Boss/Actions/Lunge")]
public class Lunge : BossAction
{
    public float speed = 10f;
    public float backSpeed = 5f;
    public float backDistance = 5f;
    private bool isLunging = false;
    public float ptwoRage = 1.5f;
    public float backDivider = 1.5f;

    public override void ExecuteAction(BossScript boss)
    {
        if (isLunging) return;
        isLunging = true;
        boss.StartCoroutine(ShortJump(boss));  
        
    }

    private IEnumerator ShortJump(BossScript boss)
    {
        boss.agent.isStopped = true; 

        yield return boss.StartCoroutine(Back(boss));
        yield return boss.StartCoroutine(Forward(boss));
        
        boss.agent.isStopped = false; 
    }

    private IEnumerator Back(BossScript boss)
    {
       Vector2 startPosition = boss.transform.position;
       Vector2 player = boss.player.position;
       Vector2 direction = (startPosition - player).normalized;
       float backMod = backDistance;
       if(boss.phaseTwoActivated)
       {
        backMod = backDistance / Mathf.Max(0.0001f, ptwoRage);
       }
       Vector2 backTartget = startPosition + direction * backMod;

       while(Vector2.Distance((Vector2)boss.transform.position, backTartget) > 0.05f)
       {
            Vector2 newPos = Vector2.MoveTowards(boss.transform.position, backTartget, backSpeed * Time.deltaTime);
            boss.transform.position = new Vector3(newPos.x, newPos.y, boss.transform.position.z);
            yield return null;
       }

    }

    private IEnumerator Forward(BossScript boss)
    {
        Vector2 lungeTarget = boss.player.position;
        float forwardMod = speed;

        if(boss.phaseTwoActivated)
        {
            forwardMod = speed * ptwoRage;
        }

        float time = 0f;
        while(time < actionDuration)
        {
            time += Time.deltaTime;
            Vector2 newPos = Vector2.MoveTowards(boss.transform.position, lungeTarget, forwardMod * Time.deltaTime);
            boss.transform.position = new Vector3(newPos.x, newPos.y, boss.transform.position.z);
            yield return null;
        }
    }
}
