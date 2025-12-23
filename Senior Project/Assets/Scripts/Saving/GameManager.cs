using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private Inventory inventorySystem;

    public EnemySpawner enemySpawner;

    //public GameObject environmentForEnemies;

    [SerializeField] private WorldClock worldClock;

    [SerializeField] private Sprite noSprite;

    public CameraShake FollowCamera;

    private Dictionary<string, Item> itemLookup = new Dictionary<string, Item>();

    public GameObject pauseMenu;
    InputAction pauseAction;

    private void Awake()
    {
        BuildItemLookup();
        gameData = new GameData();
        if (player != null)
        {
            inventorySystem = player.GetComponent<Inventory>();
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
            //DontDestroyOnLoad(gameObject);
            StartCoroutine(AutoSave());
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        inventorySystem = player.GetComponent<Inventory>();
        if (player == null)
        {
            Debug.LogWarning("GameManager: Player reference is not set.");
        }
        else if(inventorySystem == null)
        {
            Debug.LogWarning("GameManager: InventorySystem component not found on player.");
        }


        if(FollowCamera == null)
        {
            Debug.LogWarning("GameManager: Main Camera reference is not set.");
        }


        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    void Update()
    { 
        if(pauseAction.WasPressedThisFrame())
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f; // Resume game
            }
            else
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f; // Pause game
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void RestartButton()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

    public void CameraShake(float intensity, float duration)
    {
        FollowCamera.ShakeCamera(intensity, duration);
    }

    public bool IsDay() 
    {
        if (worldClock != null)
        {
            return worldClock.CurrentPhase == DayPhase.Day;
        }
        else
        {
            worldClock = FindFirstObjectByType<WorldClock>();
            if (worldClock == null)
            {
                Debug.LogWarning("GameManager: WorldClock reference is not found.");
            }

        }
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
        // Load game from previous save
        StartCoroutine(WaitAndRestart());
    }

    //Waits for 3 seconds before restarting the scene
    IEnumerator WaitAndRestart()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
                inventorySystem.slots.Clear();
                foreach (var invItem in data.inventory)
                {
                    Debug.Log($"Trying to load item: {invItem.itemName}");
                    if (itemLookup.TryGetValue(invItem.itemName, out Item item))
                    {
                        inventorySystem.slots.Add(new Slot
                        {
                            item = item,
                            amount = invItem.count
                        });
                    }
                    else
                    {
                        Debug.LogWarning($"GameManager: Item '{invItem.itemName}' not found.");
                    }
                }
                inventorySystem.RefreshUI();
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
