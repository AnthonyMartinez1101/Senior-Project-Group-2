using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


[System.Serializable]
//An Item being stored in inventory. Item + Amount
public class ItemStack
{
    public Item item;
    public int count;
    public bool IsEmpty => item == null || count <= 0;
}


public class InventorySystem : MonoBehaviour
{
    [Header("ENSURE ELEMENTS ARE NOT NULL")]
    public List<ItemStack> inventoryItems = new List<ItemStack>(1); //List of items in inventory
    public TMP_Text inventoryDisplay;


    InputAction scrollAction;

    private int inventoryIndex = 0;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(inventoryItems.Count == 0)
        {
            inventoryDisplay.text = "\nHolding no items";
        }
        else
        {
            inventoryDisplay.text = "Holding: " + inventoryItems[inventoryIndex].item.itemName + "\nCount: " + inventoryItems[inventoryIndex].count;
        }

        scrollAction = InputSystem.actions.FindAction("Scroll");
    }



    // Update is called once per frame
    void Update()
    {
        if (scrollAction != null)
        {
            //Checks scroll wheel input
            Vector2 scrollInput = scrollAction.ReadValue<Vector2>();
            if (scrollInput.y > 0.01f) ScrollUp();
            else if (scrollInput.y < -0.01f) ScrollDown();
        }
        else
        {
            Debug.LogWarning("Scroll action not found in Input System");
        }
    }

    void ScrollUp() 
    {
        inventoryIndex = (inventoryIndex + 1) % inventoryItems.Count;
        UpdateDisplayText();
    }
    void ScrollDown()
    {
        inventoryIndex = (inventoryIndex - 1 + inventoryItems.Count) % inventoryItems.Count;
        UpdateDisplayText();
    }

    void UpdateDisplayText()
    {
        if (inventoryItems.Count == 0)
        {
            inventoryDisplay.text = "\nHolding no items";
        }
        else
        {
            inventoryDisplay.text = "Holding: " + inventoryItems[inventoryIndex].item.itemName + "\nCount: " + inventoryItems[inventoryIndex].count;
        }
    }

    public void SubtractItem()
    {
        inventoryItems[inventoryIndex].count--;
    }

    public Item GetCurrentItem()
    {
        return inventoryItems[inventoryIndex].item;
    }
}
