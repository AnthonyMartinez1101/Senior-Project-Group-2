using Modern2D;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Throw Poison", menuName = "Boss/Actions/Throw Poison")]
public class ThrowPoisonAction : BossAction
{
    public GameObject projectilePrefab;
    public float throwSpeed = 15f;
    public float angleRange = 30f;

    public override void ExecuteAction(BossScript boss)
    {
        if (boss.phaseTwoActivated)
            boss.StartCoroutine(Burst(boss));
        else boss.StartCoroutine(Spread(boss));
    }

    private IEnumerator Spread(BossScript boss)
    {
        Transform player = boss.player;
        if (player == null) yield break;
        Vector2 targetPos = player.position;

        GameObject leftBall = Instantiate(projectilePrefab, boss.transform.position, Quaternion.identity);
        PoisonProjectile leftProjectile = leftBall.GetComponent<PoisonProjectile>();
        if (leftProjectile != null)
        {
            float leftAngle = Random.Range(-angleRange, -10);
            leftProjectile.Initialize(targetPos, throwSpeed, leftAngle);
        }
        GameObject rightBall = Instantiate(projectilePrefab, boss.transform.position, Quaternion.identity);
        PoisonProjectile rightProjectile = rightBall.GetComponent<PoisonProjectile>();
        if (rightProjectile != null)
        {
            float rightAngle = Random.Range(10, angleRange);
            rightProjectile.Initialize(targetPos, throwSpeed, rightAngle);
        }
        GameObject centerBall = Instantiate(projectilePrefab, boss.transform.position, Quaternion.identity);
        PoisonProjectile centerProjectile = centerBall.GetComponent<PoisonProjectile>();
        if (centerProjectile != null)
        {
            centerProjectile.Initialize(targetPos, throwSpeed, 0);
        }
        yield break;
    }

    private IEnumerator Burst(BossScript boss)
    {
        Transform player = boss.player;
        if (player == null) yield break;

        for (int i = 0; i < 5; i++)
        {
            Vector2 targetPos = player.position;

            GameObject ball = Instantiate(projectilePrefab, boss.transform.position, Quaternion.identity);
            PoisonProjectile projectile = ball.GetComponent<PoisonProjectile>();
            if (projectile != null)
            {
                float angleOffset = Random.Range(-10, 10);
                projectile.Initialize(targetPos, throwSpeed, angleOffset);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
