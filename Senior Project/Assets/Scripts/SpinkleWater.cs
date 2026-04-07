using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpinkleWater : MonoBehaviour
{
    private List <SoilScript> surroundingSoil = new List <SoilScript> ();
    private SoilScript currentSoil;

    public float cycleTime = 5f; // Time in seconds between each sprinkle cycle

    private int waterCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSoil = GetComponent<PlantScript>().GetConnectedSoil();
        StartCoroutine(SprinkleCycle());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SoilScript checkSoil = other.GetComponent<SoilScript>();
        if (checkSoil != null && checkSoil != currentSoil)
        {
            surroundingSoil.Add(checkSoil);
        }
    }

    IEnumerator SprinkleCycle()
    {
        while (true)
        {
            if(HasWater())
            {
                foreach (SoilScript soil in surroundingSoil)
                {
                    soil.Water();
                }
                waterCount--;
            }
            yield return new WaitForSeconds(cycleTime);
        }
    }

    public void FillWater()
    {
        waterCount = 2;
    }

    private bool HasWater()
    {
        return waterCount > 0;
    }
}
