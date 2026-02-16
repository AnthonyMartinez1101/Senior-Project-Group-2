using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Dig", menuName = "Boss/Actions/Dig")]
public class Dig : BossAction
{
    public Sprite warning;
    public float disappearDuration = 1f;
    public float flashDuration = 0.8f;
    public int flashCount = 3;


    public override void ExecuteAction(BossScript boss)
    {
        boss.StartCoroutine(DigCoroutine(boss));  
    }

    private IEnumerator DigCoroutine(BossScript boss)
    {
        Vector2 playerStartPos = boss.GetPlayerPosition();
        float disappearTime = disappearDuration;
        Vector3 originalPos = boss.transform.position;

        SpriteRenderer bossRenderer  = boss.GetComponent<SpriteRenderer>();
        Collider2D bossColliders = boss.GetComponent<Collider2D>();
        var agent = boss.agent;

        if(agent != null ) 
        {
          agent.enabled = false; 
        }

        if(bossRenderer != null)
        {
             bossRenderer.enabled = false;
             Debug.Log("Boss sprite renderer disabled");
        } 

        if(bossColliders != null)
        {
            bossColliders.enabled = false;
        }


        GameObject warningInstance = null;
        SpriteRenderer warningRenderer = null;

        if(warning != null)
        {
            warningInstance = new GameObject("DigWarning");
            warningInstance.transform.position = playerStartPos; //spawn the warning at the player's position
            warningRenderer = warningInstance.AddComponent<SpriteRenderer>();
            warningRenderer.sprite = warning;
            warningRenderer.enabled = false;
            Debug.Log("Warning sprite renderer created and disabled");
        }   

        float waitFlash = Mathf.Max(0, disappearTime - flashDuration); //wait time before starting to flash, the total disappear time is as long as the flashing time
        if(waitFlash > 0)
        {
            yield return new WaitForSeconds(waitFlash);
        }

        if(flashDuration > 0 && flashCount > 0 && warningRenderer != null)
        {
            float halfPeriod = flashDuration / (flashCount * 2); // each complete flash cycle needs TWO periods: one for "on" and one for "off"
            for (int i = 0; i < flashCount; i++)
            {
                warningRenderer.enabled = true; //toggle visibility
                yield return new WaitForSeconds(halfPeriod);
                warningRenderer.enabled = false; //toggle visibility
                yield return new WaitForSeconds(halfPeriod);
            }
        }

        if(warningInstance != null)
        {
            Object.Destroy(warningInstance);
        }

        Vector2 newPosition = playerStartPos;
        boss.transform.position = new Vector3(newPosition.x,newPosition.y, boss.transform.position.z); //teleport the boss to the new position. Z is used so layers are not messed up
        Debug.Log("Boss teleported to: " + newPosition);

        //re-enable the boss components after teleporting
        if(bossRenderer != null)
        {
             bossRenderer.enabled = true;
             Debug.Log("Boss sprite renderer re-enabled");
        }

        if(bossColliders != null)
        {
            bossColliders.enabled = true;
        }

        if(agent != null ) 
        {
          agent.enabled = true; 
        }

        yield break;
    }
}