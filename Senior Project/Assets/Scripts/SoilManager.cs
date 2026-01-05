using UnityEngine;

public class SoilManager : MonoBehaviour
{
    public int PlantsWatered()
    {
        int count = 0;
        SoilScript[] soils = GetComponentsInChildren<SoilScript>();
        foreach (SoilScript soil in soils)
        {
            if (soil.currentPlant != null && soil.isWatered())
            {
                count++;
            }
        }
        return count;
    }

    public int TotalPlants()
    {
        int count = 0;
        SoilScript[] soils = GetComponentsInChildren<SoilScript>();
        foreach (SoilScript soil in soils)
        {
            if (soil.currentPlant != null)
            {
                count++;
            }
        }
        return count;
    }
}
