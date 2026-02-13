using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dig", menuName = "Boss/Actions/Dig")]
public class Dig : BossAction
{
    public GameObject warning;
    public float disappearDuration = 1f;
    public float flashDuration = 0.8f;
    public int flashCount = 3;
    public float teleportDistanceMin = 1.0f;
    public float teleportDistanceMax = 2.5f;

    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(DigCoroutine(boss));  
    }

    private IEnumerator DigCoroutine(BossScript boss)
    {
        Vector2 playerStartPos = boss.GetPlayerPosition();
        float disappearTime = disappearDuration;
        Vector3 originalPos = boss.transform.position;

        //Spawn warning at player's position
        GameObject warningInstance = null;
        if(warning != null)
        {
            warningInstance = Object.Instantiate(warning, originalPos, Quaternion.identity);
        }

        SpriteRenderer[] renderers = warning.GetComponentsInChildren<SpriteRenderer>();
        Collider[] colliders = warning.GetComponentsInChildren<Collider>();

        var agent = boss.agent;
        if(agent != null )
        {
          agent.enabled = false; 
        }

        foreach (var r in renderers) r.enabled = false; //renderers are disabled immediately to prevent flashing of the boss before it disappears
        foreach (var c in renderers) c.enabled = false; //colliders are disabled immediately to prevent the boss from being hit while disappearing

        yield return null; //delete when code is done
    }
}