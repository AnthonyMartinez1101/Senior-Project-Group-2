using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //public TileManager tileManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //if (tileManager == null)
        //{
        //    tileManager = Object.FindFirstObjectByType<TileManager>();
        //}
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
}
