using NavMeshPlus.Extensions;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Follow", menuName = "Boss/Actions/Follow")]
public class Follow : BossAction
{
    public float speed = 5.0f;

    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(FollowPlayer(boss));
    }

    private IEnumerator FollowPlayer(BossScript boss)
    {
        float elapsed = 0f;
        while (elapsed < actionDuration)
        {
            elapsed += Time.deltaTime;
            if (boss.player != null) boss.agent.SetDestination(boss.player.position);
            if (boss.phaseTwoActivated) boss.agent.speed = speed * boss.data.rageMultiplier;
            else boss.agent.speed = speed;
            yield return null;
        }
        boss.agent.ResetPath();
    }
}