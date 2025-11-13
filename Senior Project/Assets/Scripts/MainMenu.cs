using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void NewButton()
    {
        SaveScript.DeleteSave();
        GameManager.loadOnStart = false;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadSceneAsync(nextSceneIndex);
    }

    public void LoadButton()
    {
        GameManager.loadOnStart = true;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadSceneAsync(nextSceneIndex);
    }   

    public void QuitButton()
    {
        Application.Quit();
    }
}
