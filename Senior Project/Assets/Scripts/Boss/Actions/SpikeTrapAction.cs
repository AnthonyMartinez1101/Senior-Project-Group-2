using Modern2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpikeTrapAction", menuName = "Boss/Actions/SpikeTrapAction")]
public class SpikeTrapAction : BossAction
{
    public float airTime = 10f;
    public List<GameObject> spikePrefabs;
    public Vector2 tpPosition;

    public Sprite warning;
    public float disappearDuration = 1f;
    public float flashDuration = 0.8f;
    public int flashCount = 3;

    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(ThrowSpikesCoroutine(boss));
    }

    private IEnumerator ThrowSpikesCoroutine(BossScript boss)
    {
        boss.transform.position = tpPosition;

        if (spikePrefabs == null || spikePrefabs.Count == 0)
        {
            Debug.LogError("SpikeTrapAction: spikePrefabs list is null or empty!");
            yield break;
        }

        int index = Random.Range(0, spikePrefabs.Count - 1);
        GameObject spikes = Instantiate(spikePrefabs[index]);
        float timer = airTime;
        while (timer > 0f)
        {
            if (boss.playerHealth.poisonCount > 0) break;
            timer -= Time.deltaTime;
            yield return null;
        }

        // Capture player position for warning and teleport target
        Vector2 playerPos = boss.GetPlayerPosition();

        GameObject warningInstance = null;
        SpriteRenderer warningRenderer = null;

        if (warning != null)
        {
            warningInstance = new GameObject("SpikeTrapWarning");
            warningInstance.transform.position = playerPos;
            warningRenderer = warningInstance.AddComponent<SpriteRenderer>();
            warningRenderer.sprite = warning;
            warningRenderer.enabled = false;
        }

        float waitFlash = Mathf.Max(0f, disappearDuration - flashDuration);
        if (waitFlash > 0f)
        {
            yield return new WaitForSeconds(waitFlash);
        }

        if (flashDuration > 0f && flashCount > 0 && warningRenderer != null)
        {
            float halfPeriod = flashDuration / (flashCount * 2);
            for (int i = 0; i < flashCount; i++)
            {
                warningRenderer.enabled = true;
                yield return new WaitForSeconds(halfPeriod);
                warningRenderer.enabled = false;
                yield return new WaitForSeconds(halfPeriod);
            }
        }

        if (warningInstance != null)
            Object.Destroy(warningInstance);

        boss.transform.position = new Vector3(playerPos.x, playerPos.y, boss.transform.position.z);
        GameManager.Instance.CameraShake(5f, 0.3f);

        Destroy(spikes);

        yield break;
    }
}