using UnityEngine;
using UnityEngine.EventSystems;

public class DeselectButton : MonoBehaviour
{
    public void Deselect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
