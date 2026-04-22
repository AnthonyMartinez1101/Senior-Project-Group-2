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

        if(boss.player != null) boss.transform.position = boss.player.position;
        GameManager.Instance.CameraShake(5f, 0.3f); //shake the camera when the boss re-appears

        Destroy(spikes);

        yield break;
    }
}