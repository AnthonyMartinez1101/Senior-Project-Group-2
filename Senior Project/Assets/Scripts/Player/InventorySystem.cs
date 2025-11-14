using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("HOTBAR UI REFERENCES")]
    public List<Image> slotBackgrounds;
    public List<TMP_Text> slotCounts;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.gray;

    public Sprite emptySlotSprite;    // A simple square (can be blank)
    public List<Image> slotIcons;     // Icons (will be disabled for no sprite)

    InputAction scrollAction;

    private int inventoryIndex = 0;

    // Ensure the current index is valid for the inventory list
    private void EnsureValidIndex()
    {
        if (inventoryItems == null || inventoryItems.Count == 0)
        {
            inventoryIndex = 0;
            return;
        }

        if (inventoryIndex < 0) inventoryIndex = 0;
        if (inventoryIndex >= inventoryItems.Count) inventoryIndex = inventoryItems.Count - 1;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scrollAction = InputSystem.actions.FindAction("Scroll");
        EnsureValidIndex();
        UpdateDisplayText();
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
        if (inventoryItems == null || inventoryItems.Count == 0) return;
        inventoryIndex = (inventoryIndex + 1) % inventoryItems.Count;
        UpdateDisplayText();
    }
    void ScrollDown()
    {
        if (inventoryItems == null || inventoryItems.Count == 0) return;
        inventoryIndex = (inventoryIndex - 1 + inventoryItems.Count) % inventoryItems.Count;
        UpdateDisplayText();
    }

    public void UpdateDisplayText()
    {
        EnsureValidIndex();

        int uiCount = Mathf.Max( (slotBackgrounds!=null?slotBackgrounds.Count:0), Mathf.Max((slotIcons!=null?slotIcons.Count:0), (slotCounts!=null?slotCounts.Count:0)) );

        for (int i = 0; i < uiCount; i++)
        {
            bool hasBackground = slotBackgrounds != null && i < slotBackgrounds.Count && slotBackgrounds[i] != null;
            bool hasIcon = slotIcons != null && i < slotIcons.Count && slotIcons[i] != null;
            bool hasCount = slotCounts != null && i < slotCounts.Count && slotCounts[i] != null;

            // Default empty visuals
            if (hasIcon) slotIcons[i].enabled = false;
            if (hasCount) slotCounts[i].text = "";
            if (hasBackground) slotBackgrounds[i].color = normalColor;

            // Fill slot from inventory if available
            if (inventoryItems != null && i < inventoryItems.Count && inventoryItems[i] != null && inventoryItems[i].item != null && inventoryItems[i].count > 0)
            {
                ItemStack stack = inventoryItems[i];

                if (hasIcon)
                {
                    if (stack.item.icon != null)
                    {
                        slotIcons[i].enabled = true;
                        slotIcons[i].sprite = stack.item.icon;
                    }
                    else
                    {
                        slotIcons[i].enabled = false;
                        if (emptySlotSprite != null) slotIcons[i].sprite = emptySlotSprite;
                    }
                }

                if (hasCount)
                {
                    slotCounts[i].text = stack.count > 1 ? stack.count.ToString() : "";
                }
            }

            // Highlight selection if this slot is the current index and we have at least one item
            if (hasBackground)
            {
                bool shouldHighlight = (inventoryItems != null && inventoryItems.Count > 0 && i == inventoryIndex);
                slotBackgrounds[i].color = shouldHighlight ? selectedColor : normalColor;
            }
        }

        // Update the display text for currently held item
        if (inventoryItems == null || inventoryItems.Count == 0)
        {
            if (inventoryDisplay != null) inventoryDisplay.text = "\nHolding no items";
        }
        else
        {
            ItemStack current = inventoryItems[inventoryIndex];
            if (inventoryDisplay != null)
                inventoryDisplay.text = "Holding: " + (current.item != null ? current.item.itemName : "(none)") + "\nCount: " + current.count;
        }
    }

    public void AddItem(Item newItem)
    {
        if(newItem == null) return;

        //Check if item already exists in inventory
        foreach (ItemStack stack in inventoryItems)
        {
            if (stack.item == newItem)
            {
                stack.count++;
                UpdateDisplayText();
                return;
            }
        }

        // Otherwise add a new stack
        inventoryItems.Add(new ItemStack { item = newItem, count = 1 });
        UpdateDisplayText();
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= inventoryItems.Count) return;
        if (indexB < 0 || indexB >= inventoryItems.Count) return;

        ItemStack temp = inventoryItems[indexA];
        inventoryItems[indexA] = inventoryItems[indexB];
        inventoryItems[indexB] = temp;

        UpdateDisplayText();
    }

    public void RefillWater()
    {
        foreach (ItemStack stack in inventoryItems)
        {
            if (stack.item != null && stack.item.itemType == ItemType.WaterCan)
            {
                stack.count = 5;
                UpdateDisplayText();
                return;
            }
        }
    }

    public void SubtractItem()
    {
        if (inventoryItems == null || inventoryItems.Count == 0) return;
        if (inventoryIndex < 0 || inventoryIndex >= inventoryItems.Count) return;

        inventoryItems[inventoryIndex].count--;
        if (inventoryItems[inventoryIndex].count <= 0)
        {
            inventoryItems[inventoryIndex].item = null;
            inventoryItems[inventoryIndex].count = 0;
        }

        UpdateDisplayText();
    }

    public Item GetCurrentItem()
    {
        if (inventoryItems == null || inventoryItems.Count == 0) return null;
        if (inventoryIndex < 0 || inventoryIndex >= inventoryItems.Count) return null;
        return inventoryItems[inventoryIndex].item;
    }

    public int GetCurrentItemCount()
    {
        if (inventoryItems == null || inventoryItems.Count == 0) return 0;
        if (inventoryIndex < 0 || inventoryIndex >= inventoryItems.Count) return 0;
        return inventoryItems[inventoryIndex].count;
    }
}
