using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Lunge", menuName = "Boss/Actions/Lunge")]
public class Lunge : BossAction
{
    //public float speed = 10f;
    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(ShortJump(boss));
    }

    private IEnumerator ShortJump(BossScript boss)
    {
        //stop the follow script? 
        boss.agent.isStopped = true; 
        float elapsed = 0f; 
        while (elapsed < actionDuration)
        {
            //agent.Move(um * speed * Time.deltaTime)
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.agent.isStopped = false; 
    }
}
