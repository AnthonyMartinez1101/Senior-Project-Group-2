using UnityEngine;
using UnityEngine.UI;

public class WaterMeter : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if(slider == null)
        {
            Debug.LogWarning("WaterMeter: No Slider component found on the GameObject.");
        }
    }

    public void SetMaxWater(int waterAmount)
    {
        slider.maxValue = waterAmount;
        slider.value = waterAmount;
    }

    public void SetWater(int waterAmount)
    {
        slider.value = waterAmount;
    }
}
