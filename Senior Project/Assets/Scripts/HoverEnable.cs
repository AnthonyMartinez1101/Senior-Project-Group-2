using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEnable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject nameBanner;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (nameBanner) nameBanner.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (nameBanner) nameBanner.SetActive(false);
    }

    void OnEnable()
    {
        if(nameBanner) nameBanner.SetActive(false);
    }
}
