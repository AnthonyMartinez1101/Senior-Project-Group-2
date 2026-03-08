using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void NewButton()
    {
        SaveScript.DeleteSave();
        GameManager.loadOnStart = false;
        SceneManager.LoadSceneAsync("Cutscene");
    }

    public void LoadButton()
    {
        GameManager.loadOnStart = true;
        SceneManager.LoadScene("Main Scene");
    }   

    public void QuitButton()
    {
        Application.Quit();
    }

    public void StartTutorial()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadSceneAsync(nextSceneIndex);
    }

    public void SkipTutorial()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 2;
        SceneManager.LoadSceneAsync(nextSceneIndex);
    }

    public void TestBossOne()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void TestBossTwo()
    {
        SceneManager.LoadSceneAsync(4);
    }

    public void TestBossThree()
    {
        SceneManager.LoadSceneAsync(5);
    }

    public void TestBossFour()
    {
        SceneManager.LoadSceneAsync(6);
    }
}
