using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void NewButton()
    {
        SaveScript.DeleteSave();
        GameManager.loadOnStart = false;
        SceneManager.LoadSceneAsync("Demo V1");
    }

    public void LoadButton()
    {
        GameManager.loadOnStart = true;
        SceneManager.LoadSceneAsync("Demo V1");
    }   

    public void QuitButton()
    {
        Application.Quit();
    }
}
