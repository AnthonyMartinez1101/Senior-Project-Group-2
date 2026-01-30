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
            boss.agent.SetDestination(boss.player.position);
            boss.agent.speed = speed;
            elapsed += Time.deltaTime;
            yield return null;
        }
        boss.agent.ResetPath();
    }
}