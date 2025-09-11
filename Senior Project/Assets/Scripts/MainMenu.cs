using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("Main Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
