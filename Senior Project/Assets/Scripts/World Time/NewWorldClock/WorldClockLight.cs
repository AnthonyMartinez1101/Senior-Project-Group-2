using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldClockLight : MonoBehaviour
{
    private Light2D worldLight;

    [SerializeField] private WorldClock worldClock;
    [SerializeField] private Gradient gradient;

    private bool transitionLight = false;

    private float transitionDuration = 5f;

    private void Awake()
    {
        worldLight = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!transitionLight)
        {
            if (worldClock != null && worldClock.CurrentPhase == DayPhase.Day)
            {
                worldLight.color = gradient.Evaluate(worldClock.PercentageOfDay() / 2f);
            }
        }
        else
        {
            if(transitionDuration >= 0)
            {
                if(worldClock.CurrentPhase == DayPhase.Day)
                {
                    worldLight.color = gradient.Evaluate(0.6f - ((transitionDuration / 5f) /10f));
                }
                else
                {
                    worldLight.color = gradient.Evaluate(1f - ((transitionDuration / 5f) / 10f));
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
}
