using UnityEngine;

public class EnvironmentChange: MonoBehaviour
{
    public WorldClock time;

    public Sprite Spring;
    public Sprite Summer;
    public Sprite Fall;
    public Sprite Winter;

    public GameObject background;

    void Start()
    {
        UpdateSeasonalDecor();
    }

    void Update()
    {
        UpdateSeasonalDecor();
    }
    public void UpdateSeasonalDecor()
    {
        switch (time.CurrentSeason)
        {
            case SeasonPhase.Spring:
                background.GetComponent<SpriteRenderer>().sprite = Spring;
                break;
            case SeasonPhase.Summer:
                background.GetComponent<SpriteRenderer>().sprite = Summer;
                break;
            case SeasonPhase.Fall:
                background.GetComponent<SpriteRenderer>().sprite = Fall;
                break;
            case SeasonPhase.Winter:
                background.GetComponent<SpriteRenderer>().sprite = Winter;
                break;
        }
    }
}
