using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    private Inventory inventory;
    private int slotIndex = -1;

    void Awake()
    {
        DragAndDrop dd = GetComponent<DragAndDrop>();
        inventory = dd.inventory;
        slotIndex = dd.slotIndex;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(inventory == null || slotIndex == -1 || eventData.pointerDrag == null) return;

        DragAndDrop dd = eventData.pointerDrag.GetComponent<DragAndDrop>();
        if(dd == null) return;

        int fromIndex = dd.slotIndex;
        
        if(fromIndex == slotIndex) return;

        inventory.CombineItems(fromIndex, slotIndex);
    }
}
