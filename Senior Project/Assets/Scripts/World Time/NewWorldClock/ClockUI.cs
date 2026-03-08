using UnityEngine;
using UnityEngine.UI;


public class ClockUI : MonoBehaviour
{
    public WorldClock worldClock; // Reference to the WorldClock script

    public GameObject hand;

    public Image seasonalDecor;

    public Sprite SpringDecor;
    public Sprite SummerDecor;
    public Sprite FallDecor;
    public Sprite WintorDecor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (worldClock == null)
        {
            Debug.LogError("WorldClock reference is not set in ClockUI.");
        }

        if (hand == null)
        {
            Debug.LogError("Hand Image reference is not set in ClockUI.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float percentage = worldClock.PercentageOfDayAndNight();
        hand.transform.rotation = Quaternion.Euler(0f, 0f, -percentage * 180f + 90f);
    }

    public void UpdateSeasonalDecor()
    {
        if (seasonalDecor == null)
        {
            Debug.LogError("Seasonal Decor reference is not set in ClockUI.");
            return;
        }
        switch (worldClock.CurrentSeason)
        {
            case SeasonPhase.Spring:
                seasonalDecor.sprite = SpringDecor;
                break;
            case SeasonPhase.Summer:
                seasonalDecor.sprite = SummerDecor; 
                break;
            case SeasonPhase.Fall:
                seasonalDecor.sprite = FallDecor; 
                break;
            case SeasonPhase.Winter:
                seasonalDecor.sprite = WintorDecor; 
                break;
        }
    }
}
