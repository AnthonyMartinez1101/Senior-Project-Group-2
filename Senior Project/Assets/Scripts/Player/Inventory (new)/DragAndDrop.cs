using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Transform originalParent;
    public Inventory inventory;
    public int slotIndex = -1;

    private Image image;

    void Start()
    {
        if (inventory == null) Debug.LogWarning("Inventory not set in slot index");
        if(slotIndex == -1) Debug.LogWarning("Slot index not set in DragAndDrop script");

        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originalParent);

        if (EventSystem.current.IsPointerOverGameObject() == false)
        {
            inventory.DropItemSlot(slotIndex);
        }

        image.raycastTarget = true;
    }
}
