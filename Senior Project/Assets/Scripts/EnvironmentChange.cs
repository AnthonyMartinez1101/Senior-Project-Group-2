using System.Collections;
using UnityEngine;

public class EnvironmentChange: MonoBehaviour
{
    public WorldClock time;

    public GameObject Spring;
    public GameObject Summer;
    public GameObject Fall;
    public GameObject Winter;

    void Update()
    {
        UpdateSeasonalDecor();
    }

    public void UpdateSeasonalDecor()
    {
        switch (time.CurrentSeason)
        {
            case SeasonPhase.Spring:
                break;
            case SeasonPhase.Summer:
                StartCoroutine(FadeAlpha(Spring.GetComponent<SpriteRenderer>()));
                break;
            case SeasonPhase.Fall:
                StartCoroutine(FadeAlpha(Summer.GetComponent<SpriteRenderer>()));
                break;
            case SeasonPhase.Winter:
                StartCoroutine(FadeAlpha(Fall.GetComponent<SpriteRenderer>()));
                break;
        }
    }


    private IEnumerator FadeAlpha(SpriteRenderer sprite)
    {
        float start = sprite.color.a; 
        float t = 0f; 
        while (t < 0.5f) 
        { 
            t += Time.deltaTime; 
            Color c = sprite.color; 
            c.a = Mathf.Lerp(start, 0, t / 0.5f); 
            sprite.color = c; 
            yield return null; 
        }
        Color final = sprite.color; 
        final.a = 0; 
        sprite.color = final;
    }
}
