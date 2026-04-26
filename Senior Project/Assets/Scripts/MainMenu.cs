using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button titleButton;
    public Button playButton;
    public Toggle settingsButton;
    private GameObject lastSelected;

    void OnEnable()
    {
        // Set the first selected button when the menu is enabled
        EventSystem.current.SetSelectedGameObject(titleButton.gameObject);
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null && lastSelected != null)
        {
            // Restore last selected UI element for keyboard navigation
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }

    public void EnablePlayScreen()
    {
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    public void EnableSettingsScreen()
    {
        EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
    }

    public void BackToMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(titleButton.gameObject);
    }

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

    public void BossOneButton()
    {
        SceneManager.LoadSceneAsync("Boss 1");
    }
    public void BossTwoButton()
    {
        SceneManager.LoadSceneAsync("Boss 2");
    }
    public void BossThreeButton()
    {
        SceneManager.LoadSceneAsync("Boss 3");
    }
    public void BossFourButton()
    {
        SceneManager.LoadSceneAsync("Boss 4");
    }
    public void BossArenaButton()
    {
        SceneManager.LoadSceneAsync("Boss Arena");
    }


    public void SkipTutorial()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 2;
        SceneManager.LoadSceneAsync(nextSceneIndex);
    }
}
