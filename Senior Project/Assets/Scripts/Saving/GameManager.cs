using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool loadOnStart = false;
    public static GameManager Instance;
    public static GameData gameData;
    public float autoSaveTimer = 10f; // Change to total time in a day/night cycle later

    public InventorySystem inventorySystem;

    public EnemySpawner enemySpawner;

    private void Awake()
    {
        if (loadOnStart)
        {
            gameData = SaveScript.LoadGame();
            if (gameData != null)
            {
                LoadData(gameData);
            }
            else
            {
                Debug.Log("No save data found. Starting new game.");
                gameData = new GameData();
                LoadData(gameData);
            }
        }
        else
        {
            gameData = new GameData();
            LoadData(gameData);
        }

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
        if(inventorySystem == null)
        {
            Debug.LogWarning("GameManager: InventorySystem reference (from player) is not set.");
        }
    }

    public bool IsDay() 
    {
        if (enemySpawner != null)
        {
            return enemySpawner.IsDay();
        }
        Debug.LogWarning("GameManager: EnemySpawner reference is not set.");
        return true; // Default to day if enemySpawner is not set
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
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                SaveScript.SaveGame(player);
            }
        }
    }

    private void LoadData(GameData data)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        }
    }
}
