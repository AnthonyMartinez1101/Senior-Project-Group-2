using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Web", menuName = "Boss/Actions/Web")]

public class WebAction : BossAction
{
    public GameObject webPrefab;
    public float throwSpeed = 15f;
    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(WebPlayer(boss));
    }

    private IEnumerator WebPlayer(BossScript boss)
    {
        Transform player = boss.player;
        if (player == null) yield break;
        Vector2 playerPos = player.position;

        GameObject webObject = Instantiate(webPrefab, boss.transform.position, Quaternion.identity);

        Rigidbody2D rb = webObject.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            Vector2 dir = (playerPos - (Vector2)boss.transform.position).normalized;
            rb.linearVelocity = dir * throwSpeed;
        }

        yield return null;
    }
}
