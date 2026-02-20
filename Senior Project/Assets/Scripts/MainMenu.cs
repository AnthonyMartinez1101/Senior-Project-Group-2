using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public GameObject TitleScreen;
    public GameObject TutorialScreen;

    public void NewButton()
    {
        SaveScript.DeleteSave();
        GameManager.loadOnStart = false;
        TitleScreen.SetActive(false);
        TutorialScreen.SetActive(true);
    }

    public void LoadButton()
    {
        GameManager.loadOnStart = true;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 2;
        SceneManager.LoadSceneAsync(nextSceneIndex);
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

    public void TestBossScene()
    {
        SceneManager.LoadSceneAsync(3);
    }

    public void BackToTitle()
    {
        TutorialScreen.SetActive(false);
        TitleScreen.SetActive(true);
    }
}
