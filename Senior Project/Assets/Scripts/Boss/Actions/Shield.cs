using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Shield", menuName = "Boss/Actions/Shield")]
public class Shield : BossAction
{
    [Tooltip("Prefab that has the ShieldAction MonoBehaviour attached")]
    public GameObject shieldPrefab;

    [Tooltip("Local offset relative to the boss when spawned")]
    public Vector3 localOffset = Vector3.zero;

    [Tooltip("Parent the shield under the boss (recommended)")]
    public bool attachToBoss = true;

    public override void ExecuteAction(BossScript boss)
    {
        if (boss.hasUsedShield)
        {
            actionDuration = 0f;
            return;
        }

        boss.hasUsedShield = true;

        actionDuration = 3f;
        boss.StartCoroutine(SpawnShieldCoroutine(boss));
    }

    private IEnumerator SpawnShieldCoroutine(BossScript boss)
    {
        if (shieldPrefab == null)
        {
            yield break;
        }

        Vector3 spawnPos = boss.transform.position + localOffset;
        GameObject shieldGo = Object.Instantiate(shieldPrefab, spawnPos, boss.transform.rotation);

        if (attachToBoss) shieldGo.transform.SetParent(boss.transform);

        yield return null;

        ShieldAction sa = shieldGo.GetComponent<ShieldAction>();
        if (sa != null)
        {
            sa.Initialize(boss);
        }
    }
}