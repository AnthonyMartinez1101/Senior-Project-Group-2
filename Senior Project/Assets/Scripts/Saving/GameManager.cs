using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using WorldTime;
using static Unity.Cinemachine.CinemachineSplineRoll;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static bool loadOnStart = false;
    public static GameManager Instance;
    public static GameData gameData;
    public float autoSaveTimer = 10f;
    public bool canSave = true;
    //add ui mabober

    public GameObject player;
    private Inventory inventorySystem;

    [SerializeField] private GameObject Enemies;
    public EnemySpawner enemySpawner;

    //public GameObject environmentForEnemies;

    [SerializeField] private WorldClock worldClock;

    [SerializeField] private Sprite noSprite;

    public CameraShake FollowCamera;

    private Dictionary<string, Item> itemLookup = new Dictionary<string, Item>();

    public ShopScript shop;

    public GameObject pauseMenu;
    InputAction pauseAction;

    public GameObject chickenPrefab;
    public GameObject GameOverScreen;

    [SerializeField] private Transform chickenHidePosition;

    public Image blackBackground;


    private void Awake()
    {
        GameOverScreen.SetActive(false);

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
            if (canSave)
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


        if (blackBackground == null) Debug.LogWarning("GameManager: BlackBackground reference is not set.");
        else
        {
            blackBackground.gameObject.SetActive(true);
            Color c = blackBackground.color;
            blackBackground.color = new Color(c.r, c.g, c.b, 1f);

            StartCoroutine(FadeOut(blackBackground, 3f));
        }
    }

    void Update()
    { 
        if(pauseAction.WasPressedThisFrame() && !shop.IsShopInUse())
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

    public GameObject GetPlayer()
    {
        return player;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void RestartButton()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //make game over screen pop up 
    }

    public void GoToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene(0);
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
            return worldClock.IsDay();
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

    public void AddToInventory(Item item, int runtimeData)
    {
        if (inventorySystem != null)
        {
            inventorySystem.AddItem(item, runtimeData);
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

    public void GameOverScene()
    {
        // Load game from previous save
        StartCoroutine(WaitOnDeath());
    }

    //Waits for 3 seconds before restarting the scene
    IEnumerator WaitOnDeath()
    {
        yield return new WaitForSeconds(3f);
        GameOverScreen.SetActive(true); 
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            if (Enemies.transform.childCount == 0 && player != null)
            {
                yield return new WaitForSeconds(autoSaveTimer);
                bool saveEnabled = true;
                while (saveEnabled)
                {
                    if (Enemies != null && Enemies.transform.childCount == 0)
                    {
                        SaveScript.SaveGame(player);
                        saveEnabled = false;
                        Debug.Log("Game auto-saved.");
                    }
                    yield return new WaitForSeconds(1f);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void BuildItemLookup()
    {
        Debug.Log("Building item lookup...");
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
                inventorySystem.ClearInventory();
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

            if (data.coinStash > 0)
            {
                player.GetComponent<PlayerWallet>().ClearWallet();
                player.GetComponent<PlayerWallet>().AddCoins(data.coinStash);
            }

            // load item drops

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
                                plant.Create(plantItem, soil);
                                plant.currentGrowth = soilPlot.plant.growth;
                                plant.currentHealth = soilPlot.plant.health;
                                soil.currentPlant = plant;
                            }
                            else
                            {
                                //Debug.LogWarning($"PlantItem '{soilPlot.plant.plantName}' not found.");
                            }
                        }
                    }
                }
            }

            if (data.currentSeason > 0)
                worldClock.IterateSeason(data.currentSeason);
            // load specific time in day

            GameObject parent = GameObject.FindGameObjectWithTag("Chickens");
            if (data.chickens != null && data.chickens.Count > 0)
            {
                foreach (var chickenData in data.chickens)
                {
                    Vector3 position = new Vector3(chickenData.position[0], chickenData.position[1], chickenData.position[2]);
                    Instantiate(chickenPrefab, position, Quaternion.identity, parent.transform);
                }
            }
        }
    }
    
    public Sprite NoSprite()
    {
        return noSprite;
    }

    public WorldClock GetWorldClock()
    {
        return worldClock;
    }

    public Transform GetChickenHidePosition()
    {
        return chickenHidePosition;
    }

    IEnumerator FadeOut(Image image, float duration)
    {
        yield return new WaitForSeconds(0.5f); // Optional delay before starting the fade

        float elapsedTime = 0f;
        Color originalColor = image.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Ensure it's fully transparent at the end
    }
}
