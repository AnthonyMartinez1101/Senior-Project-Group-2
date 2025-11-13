using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldTime;
using static Unity.Cinemachine.CinemachineSplineRoll;

public class GameManager : MonoBehaviour
{
    public static bool loadOnStart = false;
    public static GameManager Instance;
    public static GameData gameData;
    public float autoSaveTimer = 10f; // Change to total time in a day/night cycle later

    public GameObject player;
    private InventorySystem inventorySystem;

    public EnemySpawner enemySpawner;

    public GameObject environmentForEnemies;

    [SerializeField] private WorldTime.WorldTime worldTime;

    [SerializeField] private Sprite noSprite;

    public CameraShake FollowCamera;

    private Dictionary<string, Item> itemLookup = new Dictionary<string, Item>();

    private void Awake()
    {
        BuildItemLookup();
        gameData = new GameData();
        if (player != null)
        {
            inventorySystem = player.GetComponent<InventorySystem>();
        }
        if (loadOnStart)
        {
            gameData = SaveScript.LoadGame();
            if (gameData == null)
            {
                Debug.Log("No save data found. Starting new game.");
                gameData = new GameData();
            }
        }
        LoadData(gameData);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(AutoSave());
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        inventorySystem = player.GetComponent<InventorySystem>();
        if (player == null)
        {
            Debug.LogWarning("GameManager: Player reference is not set.");
        }
        else if(inventorySystem == null)
        {
            Debug.LogWarning("GameManager: InventorySystem component not found on player.");
        }

        if(environmentForEnemies != null)
        {
            environmentForEnemies.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameManager: EnvironmentForEnemies reference is not set.");
        }

        if(FollowCamera == null)
        {
            Debug.LogWarning("GameManager: Main Camera reference is not set.");
        }
    }

    public void CameraShake(float intensity, float duration)
    {
        FollowCamera.ShakeCamera(intensity, duration);
    }

    public bool IsDay() 
    {
        if (worldTime != null)
        {
            return worldTime.CurrentPhase == Phase.Day;
        }
        Debug.LogWarning("GameManager: WorldTime reference is not set.");
        return true; // Default to day if worldTime is not set
    }

    public void AddToInventory(Item item)
    {
        if (inventorySystem != null)
        {
            inventorySystem.AddItem(item);
        }
    }

    public void RestartScene()
    {
        // // Load game from previous save
        // gameData = SaveScript.LoadGame();
        // LoadData(gameData);
        StartCoroutine(WaitAndRestart());
    }

    // Won't need this later
    //Waits for 3 seconds before restarting the scene
    IEnumerator WaitAndRestart()
    {
        yield return new WaitForSeconds(3f);
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimer);
            if (player)
            {
                SaveScript.SaveGame(player);
                Debug.Log("Game auto-saved.");
            }
        }
    }

    private void BuildItemLookup()
    {
        Item[] items = Resources.LoadAll<Item>("Objects (Scriptable Objects)/Items");
        foreach (var item in items)
        {
            itemLookup[item.itemName] = item;
            Debug.Log($"Loaded item: {item.itemName}");
        }
    }

    private SoilScript FindSoil(float[] pos)
    {
        Vector3 position = new Vector3(pos[0], pos[1], pos[2]);
        foreach (var soil in FindObjectsByType<SoilScript>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(soil.transform.position, position) < 0.1f)
            {
                return soil;
            }
        }
        return null;
    }

    private void LoadData(GameData data)
    {
        if (player != null)
        {
            player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            
            player.GetComponent<PlayerHealth>().currentHealth = data.health;

            if (data.inventory != null)
            {
                Debug.Log("clearing inventory and loading items");
                inventorySystem.inventoryItems.Clear();
                foreach (var invItem in data.inventory)
                {
                    Debug.Log($"Trying to load item: {invItem.itemName}");
                    if (itemLookup.TryGetValue(invItem.itemName, out Item item))
                    {
                        inventorySystem.inventoryItems.Add(new ItemStack
                        {
                            item = item,
                            count = invItem.count
                        });
                    }
                    else
                    {
                        Debug.LogWarning($"GameManager: Item '{invItem.itemName}' not found.");
                    }
                }
                inventorySystem.UpdateDisplayText();
            }

            if (data.soils != null)
            {
                foreach (var soilPlot in data.soils)
                {
                    SoilScript soil = FindSoil(soilPlot.position);
                    if (soil != null)
                    {
                        soil.waterLevel = soilPlot.waterLevel;
                        if (soilPlot.plant != null)
                        {
                            PlantItem plantItem = Resources.Load<PlantItem>("Objects (Scriptable Objects)/Plants/" + soilPlot.plant.plantName);
                            if (plantItem != null)
                            {
                                PlantScript plant = Instantiate(soil.plantActor, soil.transform.position, Quaternion.identity, soil.transform);
                                plant.Create(plantItem);
                                plant.currentGrowth = soilPlot.plant.growth;
                                plant.currentHealth = soilPlot.plant.health;
                                soil.currentPlant = plant;
                            }
                            else
                            {
                                Debug.LogWarning($"PlantItem '{soilPlot.plant.plantName}' not found.");
                            }
                        }
                    }
                }
            }
        }
    }
    
    public Sprite NoSprite()
    {
        return noSprite;
    }
}
