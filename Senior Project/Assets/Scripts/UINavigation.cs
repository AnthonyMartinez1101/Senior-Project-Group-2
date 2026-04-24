using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UINavigation : MonoBehaviour
{
    public Button shopButton;
    public Button pauseButton;
    public Toggle settingsButton;
    public Button gameOverButton;
    public Button winButton;
    public Button statsButton;

    private GameObject lastSelected;

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

    public void shopOpened()
    {
        EventSystem.current.SetSelectedGameObject(shopButton.gameObject);
    }

    public void pauseOpened()
    {
        EventSystem.current.SetSelectedGameObject(pauseButton.gameObject);
    }

    public void settingsOpened()
    {
        EventSystem.current.SetSelectedGameObject(settingsButton.gameObject);
    }

    public void gameOverOpened()
    {
        EventSystem.current.SetSelectedGameObject(gameOverButton.gameObject);
    }

    public void winOpened()
    {
        EventSystem.current.SetSelectedGameObject(winButton.gameObject);
    }

    public void statsOpened()
    {
        EventSystem.current.SetSelectedGameObject(statsButton.gameObject);
    }
}