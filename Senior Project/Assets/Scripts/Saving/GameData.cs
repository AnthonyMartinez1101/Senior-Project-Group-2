using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float[] position;
    public float health;
    public List<InventoryItem> inventory;
    public List<Soil> soils;

    // Todo: num days, chickens, etc

    // Default constructor for new game
    public GameData()
    {
        position = new float[3];
        position[0] = 0f;
        position[1] = 0f;
        position[2] = 0f;

        health = 20f;
    }

    // Constructor to load game data
    public GameData(GameObject player)
    {
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        health = player.GetComponent<PlayerHealth>().currentHealth;

        inventory = new List<InventoryItem>();
        foreach (var itemStack in player.GetComponent<InventorySystem>().inventoryItems)
        {
            if (!itemStack.IsEmpty)
            {
                inventory.Add(new InventoryItem(itemStack.item.itemName, itemStack.count));
            }
        }

        soils = new List<Soil>();
        SoilScript[] soilScripts = Object.FindObjectsByType<SoilScript>(FindObjectsSortMode.None);
        foreach (var soil in soilScripts)
        {
            soils.Add(new Soil(soil));
        }
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