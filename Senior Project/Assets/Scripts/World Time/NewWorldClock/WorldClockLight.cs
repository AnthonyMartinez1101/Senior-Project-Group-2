using NUnit.Framework;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;


public class WorldClockLight : MonoBehaviour
{
    private Light2D worldLight;

    [SerializeField] private WorldClock worldClock;
    [SerializeField] private Gradient springGradient;
    [SerializeField] private Gradient summerGradient;
    [SerializeField] private Gradient fallGradient;
    [SerializeField] private Gradient winterGradient;

    private Gradient currentGradient;

    private bool transitionLight = false;

    private float transitionDuration = 5f;

    private void Awake()
    {
        worldLight = GetComponent<Light2D>();
        currentGradient = springGradient;
    }

    // Update is called once per frame
    void Update()
    {
        if (!transitionLight)
        {
            if (worldClock != null && worldClock.CurrentPhase == DayPhase.Day)
            {
                worldLight.color = currentGradient.Evaluate(worldClock.PercentageOfDay() / 2f);
            }
        }
        else
        {
            if (transitionDuration >= 0)
            {
                if (worldClock.CurrentPhase == DayPhase.Day)
                {
                    worldLight.color = currentGradient.Evaluate(0.6f - ((transitionDuration / 5f) / 10f));
                }
                else
                {
                    worldLight.color = currentGradient.Evaluate(1f - ((transitionDuration / 5f) / 10f));
                }
                transitionDuration -= Time.deltaTime;
            }
            else
            {
                transitionLight = false;
                transitionDuration = 5f;
            }
        }
    }


    public void TransitionLight()
    {
        transitionLight = true;
    }


    public void SetGradient()
    {
        switch (worldClock.CurrentSeason)
        {
            case SeasonPhase.Spring:
                currentGradient = springGradient;
                break;
            case SeasonPhase.Summer:
                currentGradient = summerGradient;
                break;
            case SeasonPhase.Fall:
                currentGradient = fallGradient;
                break;
            case SeasonPhase.Winter:
                currentGradient = winterGradient;
                break;
        }
    }

    // Use during tutorial to set light to dark immediately
    public void nightLight()
    {
        transitionLight = false;
        transitionDuration = 5f;
        worldLight.color = currentGradient.Evaluate(0.7f);
    }
}
