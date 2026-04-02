using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Web", menuName = "Boss/Actions/Web")]

public class WebAction : BossAction
{
    public GameObject webPrefab;
    public float throwSpeed = 15f;
    public float stopDistance = 0.1f;
    public override void ExecuteAction(BossScript boss)
    {
        Instantiate(webPrefab, boss.transform.position, Quaternion.identity);
    }

    //private IEnumerator WebPlayer(BossScript boss)
    //{
    //    Transform player = boss.player;
    //    if (player == null) yield break;
    //    Vector2 playerPos = player.position;

    //    GameObject webObject = Instantiate(webPrefab, boss.transform.position, Quaternion.identity);

    //    while(webObject != null)
    //    {
    //      webObject.transform.position = Vector2.MoveTowards(webObject.transform.position, playerPos, throwSpeed * Time.deltaTime);

    //        float distToPlayer = Vector2.Distance(webObject.transform.position, playerPos);

    //    }

    //        yield return null;
    //}
}
