using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float[] position = new float[3];
    public float health;
    public List<InventoryItem> inventory = new List<InventoryItem>();
    public int coinStash;
    // item drops list
    public List<Soil> soils = new List<Soil>();
    public int currentSeason = 0; // 0 = spring, 1 = summer, 2 = fall, 3 = winter

    // Default constructor for new game
    public GameData()
    {
        position[0] = 2f;
        position[1] = 5f;
        position[2] = 0f;
        health = 20f;
    }

    // Constructor to load game data
    public GameData(GameObject player)
    {
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        health = player.GetComponent<PlayerHealth>().currentHealth;

        foreach (var slot in player.GetComponent<Inventory>().slots)
        {
            if (!slot.IsEmpty())
            {
                inventory.Add(new InventoryItem(slot.item.itemName, slot.amount));
            }
        }

        coinStash = player.GetComponent<PlayerWallet>().GetCoinCount();

        // item drops

        SoilScript[] soilScripts = Object.FindObjectsByType<SoilScript>(FindObjectsSortMode.None);
        foreach (var soil in soilScripts)
        {
            soils.Add(new Soil(soil));
        }

        // turret

        WorldClock worldClock = Object.FindFirstObjectByType<WorldClock>();
        currentSeason = (int)worldClock.CurrentSeason;
        // time of day

        // save chicken position

    }
}

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public int count;

    public InventoryItem(string n, int c)
    {
        itemName = n;
        count = c;
    }
}

[System.Serializable]
public class Soil
{
    public float[] position;
    public float waterLevel;
    public PlantData plant;
    public Soil(SoilScript s)
    {
        position = new float[3];
        position[0] = s.transform.position.x;
        position[1] = s.transform.position.y;
        position[2] = s.transform.position.z;
        waterLevel = s.waterLevel;
        plant = null;
        if (s.currentPlant != null)
        {
            plant = new PlantData(s.currentPlant);
        }
    }
}

[System.Serializable]
public class PlantData
{
    public string plantName;
    public float growth;
    public float health;
    public PlantData(PlantScript p)
    {
        plantName = p.plantInfo.plantName;
        growth = p.currentGrowth;
        health = p.currentHealth;
    }
}