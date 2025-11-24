using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

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
    //public TMP_Text inventoryDisplay;

    [Header("HOTBAR UI REFERENCES")]
    [SerializeField] private List<Image> slotBackgrounds = new List<Image>();
    [SerializeField] private List<TMP_Text> slotCounts = new List<TMP_Text>();
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.gray;

    public Sprite emptySlotSprite;    // A simple square (can be blank)
    [SerializeField] private List<Image> slotIcons = new List<Image>();     // Icons (will be disabled for no sprite)
    
    [Header("HOTBAR SETTINGS")]
    public int hotbarSize = 9;  // Number of hotbar slots
    public bool allowNumberKeySelection = true;  // Enable 1-9 key selection

    private InputAction scrollAction;
    private InputAction numberKeysAction;

    private int inventoryIndex = 0;
    
    // Event called when hotbar selection changes
    [HideInInspector] public UnityEvent<int> OnHotbarSlotChanged = new UnityEvent<int>();
    [HideInInspector] public UnityEvent<ItemStack> OnHeldItemChanged = new UnityEvent<ItemStack>();

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
    
    /// <summary>
    /// Adds a UI slot reference to the hotbar. Call this during setup.
    /// </summary>
    public void AddHotbarSlot(Image background, TMP_Text count, Image icon)
    {
        if (slotBackgrounds == null) slotBackgrounds = new List<Image>();
        if (slotCounts == null) slotCounts = new List<TMP_Text>();
        if (slotIcons == null) slotIcons = new List<Image>();
        
        slotBackgrounds.Add(background);
        slotCounts.Add(count);
        slotIcons.Add(icon);
    }
    
    /// <summary>
    /// Clears all UI slot references
    /// </summary>
    public void ClearHotbarSlots()
    {
        if (slotBackgrounds != null) slotBackgrounds.Clear();
        if (slotCounts != null) slotCounts.Clear();
        if (slotIcons != null) slotIcons.Clear();
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
        
        // Check for number key input (1-9)
        if (allowNumberKeySelection)
        {
            HandleNumberKeySelection();
        }
    }
    
    void HandleNumberKeySelection()
    {
        // Using the new Input System - check keyboard input safely
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard == null) return;
        
        for (int i = 0; i < 9; i++)
        {
            // Map 0-8 to keys 1-9
            UnityEngine.InputSystem.Key key = UnityEngine.InputSystem.Key.Digit1 + i;
            if (keyboard[key].wasPressedThisFrame)
            {
                if (i < Mathf.Min(hotbarSize, inventoryItems.Count))
                {
                    SetHotbarSlot(i);
                }
            }
        }
    }

    void ScrollUp()
    {
        if (inventoryItems == null || inventoryItems.Count == 0) return;
        inventoryIndex = (inventoryIndex + 1) % Mathf.Min(hotbarSize, inventoryItems.Count);
        UpdateDisplayText();
    }
    void ScrollDown()
    {
        if (inventoryItems == null || inventoryItems.Count == 0) return;
        int maxSlots = Mathf.Min(hotbarSize, inventoryItems.Count);
        inventoryIndex = (inventoryIndex - 1 + maxSlots) % maxSlots;
        UpdateDisplayText();
    }
    
    /// <summary>
    /// Sets the current hotbar slot by index (0-based)
    /// </summary>
    public void SetHotbarSlot(int slotIndex)
    {
        if (inventoryItems == null || inventoryItems.Count == 0) return;
        if (slotIndex < 0 || slotIndex >= Mathf.Min(hotbarSize, inventoryItems.Count)) return;
        
        inventoryIndex = slotIndex;
        OnHotbarSlotChanged.Invoke(slotIndex);
        UpdateDisplayText();
    }
    
    /// <summary>
    /// Gets the current hotbar slot index
    /// </summary>
    public int GetCurrentHotbarSlot()
    {
        return inventoryIndex;
    }

    public void UpdateDisplayText()
    {
        EnsureValidIndex();

        int uiCount = Mathf.Max( (slotBackgrounds!=null?slotBackgrounds.Count:0), Mathf.Max((slotIcons!=null?slotIcons.Count:0), (slotCounts!=null?slotCounts.Count:0)) );
        int maxSlots = Mathf.Min(hotbarSize, uiCount);

        for (int i = 0; i < maxSlots; i++)
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
        
        // Disable remaining UI slots beyond hotbar size
        for (int i = maxSlots; i < uiCount; i++)
        {
            if (slotBackgrounds != null && i < slotBackgrounds.Count && slotBackgrounds[i] != null)
                slotBackgrounds[i].color = normalColor;
            if (slotIcons != null && i < slotIcons.Count && slotIcons[i] != null)
                slotIcons[i].enabled = false;
            if (slotCounts != null && i < slotCounts.Count && slotCounts[i] != null)
                slotCounts[i].text = "";
        }

        // Update the display text for currently held item
        //if (inventoryItems == null || inventoryItems.Count == 0)
        //{
        //    if (inventoryDisplay != null) inventoryDisplay.text = "\nHolding no items";
        //}
        //else
        //{
        //    ItemStack current = inventoryItems[inventoryIndex];
        //    if (inventoryDisplay != null)
        //        inventoryDisplay.text = "Holding: " + (current.item != null ? current.item.itemName : "(none)") + "\nCount: " + current.count;
            
        //    // Invoke event when held item changes
        //    OnHeldItemChanged.Invoke(current);
        //}
        
        // Debug: Log current inventory state
        if (inventoryItems != null && inventoryItems.Count > 0)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].item != null)
                {
                    bool hasIcon = inventoryItems[i].item.icon != null;
                    string slotMarker = i == inventoryIndex ? ">>>" : "   ";
                    //Debug.Log($"[Hotbar {slotMarker}] Slot {i}: {inventoryItems[i].item.itemName} x{inventoryItems[i].count} | Icon: {(hasIcon ? "✓" : "❌")}");
                }
            }
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
    
    /// <summary>
    /// Subtracts from a specific item's count without removing it from inventory.
    /// Useful for non-consumable items like the watering can.
    /// </summary>
    public void SubtractItemAtSlot(int slotIndex, int amount = 1)
    {
        if (inventoryItems == null || slotIndex < 0 || slotIndex >= inventoryItems.Count) return;
        
        inventoryItems[slotIndex].count -= amount;
        
        // Clamp to 0 (don't go negative)
        if (inventoryItems[slotIndex].count < 0)
            inventoryItems[slotIndex].count = 0;
        
        UpdateDisplayText();
    }
    
    /// <summary>
    /// Adds to a specific item's count. Useful for refilling non-consumable items.
    /// </summary>
    public void AddToItemAtSlot(int slotIndex, int amount = 1)
    {
        if (inventoryItems == null || slotIndex < 0 || slotIndex >= inventoryItems.Count) return;
        
        inventoryItems[slotIndex].count += amount;
        UpdateDisplayText();
    }

    public void SubtractItem()
    {
        if (inventoryItems == null || inventoryItems.Count == 0) return;
        if (inventoryIndex < 0 || inventoryIndex >= inventoryItems.Count) return;

        inventoryItems[inventoryIndex].count--;
        
        // Only remove the item if it's consumable and count reaches 0
        if (inventoryItems[inventoryIndex].count <= 0)
        {
            if (inventoryItems[inventoryIndex].item != null && inventoryItems[inventoryIndex].item.isConsumable)
            {
                // Consumable item - remove it from inventory
                inventoryItems[inventoryIndex].item = null;
                inventoryItems[inventoryIndex].count = 0;
            }
            else
            {
                // Non-consumable item (like watering can) - keep it, just set count to 0
                inventoryItems[inventoryIndex].count = 0;
            }
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
    
    /// <summary>
    /// Gets an item at a specific hotbar slot
    /// </summary>
    public Item GetItemAtSlot(int slotIndex)
    {
        if (inventoryItems == null || slotIndex < 0 || slotIndex >= inventoryItems.Count) return null;
        return inventoryItems[slotIndex].item;
    }
    
    /// <summary>
    /// Gets the count of an item at a specific hotbar slot
    /// </summary>
    public int GetItemCountAtSlot(int slotIndex)
    {
        if (inventoryItems == null || slotIndex < 0 || slotIndex >= inventoryItems.Count) return 0;
        return inventoryItems[slotIndex].count;
    }
    
    /// <summary>
    /// Checks if a hotbar slot is empty
    /// </summary>
    public bool IsSlotEmpty(int slotIndex)
    {
        if (inventoryItems == null || slotIndex < 0 || slotIndex >= inventoryItems.Count) return true;
        return inventoryItems[slotIndex].IsEmpty;
    }
}
