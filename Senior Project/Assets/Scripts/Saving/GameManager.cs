using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static GameData gameData;
    public float autoSaveTimer = 10f;

    private void Awake()
    {
        gameData = SaveScript.LoadGame();
        if (gameData != null)
        {
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
    }

    public void RestartScene()
    {
        StartCoroutine(WaitAndRestart());
    }

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
