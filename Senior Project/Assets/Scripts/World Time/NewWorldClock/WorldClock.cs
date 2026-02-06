using Modern2D;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public enum DayPhase
{
    Day,
    Night
}

public enum SeasonPhase
{
    Spring,
    Summer,
    Fall,
    Winter
}


public class WorldClock : MonoBehaviour
{
    [Header("Phase display lengths (seconds)")]
    [SerializeField] private float dayLength = 180f;   // 3:00 shown
    [SerializeField] private float nightLength = 120f; // 2:00 shown

    [Header("Hold at 0:00 after phase switch (seconds)")]
    [SerializeField] private float transitionLength = 5f;    // stays showing 0:00



    private float currentTime;
    private float preciseTime;

    public DayPhase CurrentPhase { get; private set; } = DayPhase.Day;
    public SeasonPhase CurrentSeason { get; private set; } = SeasonPhase.Spring;

    [Header("Other attributes:")]
    [SerializeField] private bool pauseTimer = false;
    [SerializeField] private bool _2xTickSpeed = false;

    public WorldClockLight worldClockLight;

    [SerializeField] private TMP_Text displayText;

    public bool inTutorialMode = false;

    public UnityEvent DayChangeEvent;

    private LightingSystem lightingSystem;
    private float shadowAlpha = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = dayLength;
        StartCoroutine(TickTime());

        preciseTime = dayLength;

        Debug.Log("Current Season: " + CurrentSeason.ToString());

        if (inTutorialMode)
        {
            CurrentPhase = DayPhase.Night;
            PauseTimer();
            worldClockLight.nightLight();
        }

        lightingSystem = LightingSystem.system;
        shadowAlpha = lightingSystem._shadowAlpha;
    }

    void Update()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        displayText.text = $"{minutes:0}:{seconds:00}";

        preciseTime -= Time.deltaTime;
    }

    IEnumerator TickTime()
    {
        while (true)
        {
            if (pauseTimer)
            {
                yield return null;
            }
            else
            {
                if (_2xTickSpeed) yield return new WaitForSeconds(0.5f);
                else yield return new WaitForSeconds(1f);
                currentTime -= 1f;

                if (currentTime == 0f)
                {
                    yield return StartCoroutine(TimeChange());
                }
            }
        }
    }

    public IEnumerator TimeChange()
    {
        SwitchLight();
        yield return new WaitForSeconds(transitionLength);
        SwitchPhase();
    }

    void SwitchLight()
    {
        worldClockLight.TransitionLight();
    }

    void SwitchPhase()
    {
        if (CurrentPhase == DayPhase.Day)
        {
            CurrentPhase = DayPhase.Night;
            currentTime = nightLength;
            lightingSystem._shadowAlpha.value = Mathf.Lerp(lightingSystem._shadowAlpha.value, 0, 0.5f);
        }
        else
        {
            CurrentPhase = DayPhase.Day;
            currentTime = dayLength;
            preciseTime = dayLength;
            if (!inTutorialMode)
                IterateSeason();
            lightingSystem._shadowAlpha.value = Mathf.Lerp(lightingSystem._shadowAlpha.value, shadowAlpha, 0.5f);
        }
        DayChangeEvent.Invoke();
    }

    public void IterateSeason()
    {
        int nextSeason = ((int)CurrentSeason + 1) % 4;
        CurrentSeason = (SeasonPhase)nextSeason;
        worldClockLight.SetGradient();
        Debug.Log("Current Season: " + CurrentSeason.ToString());
    }

    public void PauseTimer()
    {
        pauseTimer = true;
    }

    public void ResumeTimer()
    {
        pauseTimer = false;
    }

    public float PercentageOfDay()
    {
        return (dayLength - preciseTime) / dayLength;
    }

    public void ResetDay()
    {
        CurrentPhase = DayPhase.Day;
        currentTime = dayLength;
        preciseTime = dayLength;
        SwitchLight();
    }

    public bool IsDay()
    {
        return CurrentPhase == DayPhase.Day;
    }

    public bool IsNight()
    {
        return CurrentPhase == DayPhase.Night;
    }
}
