# Hotbar UI Setup Guide

This guide explains how to properly set up the Unity hierarchy and link the Hotbar UI with the InventorySystem script.

---

## Overview

The hotbar system consists of:
- **InventorySystem.cs** - Core inventory and hotbar logic
- **UI Canvas** - Container for hotbar display
- **Hotbar Slots** - Individual slot UI elements (background, icon, count text)

---

## Step 1: Scene Hierarchy Setup

Create the following hierarchy in your scene:

```
Canvas (UI Canvas)
├── InventoryDisplay (TextMeshPro Text)
└── Hotbar (Panel)
    ├── Slot_1 (Panel/Button)
    │   ├── Background (Image)
    │   ├── Icon (Image)
    │   └── Count (TextMeshPro Text)
    ├── Slot_2 (Panel/Button)
    │   ├── Background (Image)
    │   ├── Icon (Image)
    │   └── Count (TextMeshPro Text)
    ├── Slot_3 (Panel/Button)
    ...
    └── Slot_9 (Panel/Button)
        ├── Background (Image)
        ├── Icon (Image)
        └── Count (TextMeshPro Text)
```

---

## Step 2: Create the Canvas (if not already present)

1. Right-click in Hierarchy → UI → Canvas
2. Set Canvas Scaler to "Scale with Screen Size" for responsive design
3. Rename to "Canvas" for clarity

---

## Step 3: Create Inventory Display Text

1. Inside Canvas, create: UI → TextMeshPro - Text
2. Rename to "InventoryDisplay"
3. Position it where you want the item info shown (e.g., top-left or bottom-left)
4. Set size to approximately 200×100

---

## Step 4: Create the Hotbar Panel

1. Inside Canvas, create: UI → Panel
2. Rename to "Hotbar"
3. Configure its layout:
   - Set Rect Transform to position at bottom of screen
   - Set width to ~540 (for 9 slots × 60 pixels each)
   - Set height to ~100
   - Add a Layout Group: Grid Layout Group
     - Padding: 10 on all sides
     - Cell Size: 60×60
     - Spacing: 10×10
     - Child Controls: Check Width & Height
     - Child Force Expand: Uncheck

---

## Step 5: Create Individual Hotbar Slots

Repeat this for slots 1-9:

1. **Create Slot Panel**
   - Inside Hotbar, create: UI → Panel
   - Rename to "Slot_1", "Slot_2", etc.
   - Layout group will auto-size them

2. **Add Background Image Component**
   - Add component: Image
   - Set color to normalColor (gray by default)
   - Assign a simple sprite (white square or similar)

3. **Create Icon Image**
   - Inside Slot_X, create: UI → Image
   - Rename to "Icon"
   - Position it to fill the slot
   - Set Image Type to "Simple"
   - This will display the item sprite

4. **Create Count Text**
   - Inside Slot_X, create: UI → TextMeshPro - Text
   - Rename to "Count"
   - Position to bottom-right corner of slot
   - Set font size to 24
   - Set color to white

**Result for each slot:**
```
Slot_1 (Panel - with Image component as Background)
├── Icon (Image)
└── Count (TextMeshPro Text)
```

---

## Step 6: Create a HotbarManager Script (Optional but Recommended)

Create a script called `HotbarUIManager.cs` in your Scripts folder:

```csharp
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HotbarUIManager : MonoBehaviour
{
    public InventorySystem inventorySystem;
    public Transform hotbarContainer;  // The Hotbar panel
    
    private void Start()
    {
        if (inventorySystem == null)
        {
            inventorySystem = GetComponent<InventorySystem>();
        }
        
        // Auto-populate hotbar slots
        PopulateHotbarSlots();
        
        // Subscribe to events
        inventorySystem.OnHotbarSlotChanged.AddListener(OnSlotChanged);
        inventorySystem.OnHeldItemChanged.AddListener(OnItemChanged);
    }
    
    private void PopulateHotbarSlots()
    {
        // Clear existing references
        inventorySystem.ClearHotbarSlots();
        
        // Find all slot panels
        Image[] slots = hotbarContainer.GetComponentsInChildren<Image>();
        
        int slotIndex = 0;
        foreach (Transform slotTransform in hotbarContainer)
        {
            if (slotIndex >= inventorySystem.hotbarSize) break;
            
            Image background = slotTransform.GetComponent<Image>();
            Image icon = slotTransform.Find("Icon")?.GetComponent<Image>();
            TMP_Text count = slotTransform.Find("Count")?.GetComponent<TMP_Text>();
            
            if (background != null && icon != null && count != null)
            {
                inventorySystem.AddHotbarSlot(background, count, icon);
                slotIndex++;
            }
        }
        
        inventorySystem.UpdateDisplayText();
    }
    
    private void OnSlotChanged(int newSlot)
    {
        // Add visual effects here (animations, sounds, etc.)
        Debug.Log($"Switched to slot {newSlot + 1}");
    }
    
    private void OnItemChanged(ItemStack newItem)
    {
        // Add effects when item changes
        if (newItem.item != null)
            Debug.Log($"Now holding: {newItem.item.itemName}");
    }
}
```

---

## Step 7: Assign References

1. **Create a Player GameObject** (or use existing)
   - Add the InventorySystem script component
   - Add the HotbarUIManager script component

2. **In the Inspector, assign:**

   **InventorySystem component:**
   - Inventory Display: Drag the "InventoryDisplay" text object
   - Hotbar Size: 9 (or desired number)
   - Allow Number Key Selection: ✓ (checked)
   - Empty Slot Sprite: Assign a white square sprite (optional)

   **HotbarUIManager component:**
   - Inventory System: Drag your Player object (it will auto-find the component)
   - Hotbar Container: Drag the "Hotbar" panel

---

## Step 8: Test the Setup

1. Play the scene
2. Add some items to inventory via code or inspector:
   ```csharp
   // In Start() or via a test button
   inventorySystem.AddItem(someItem);
   ```
3. Test controls:
   - Scroll mouse wheel to change slots
   - Press 1-9 keys to switch slots directly
   - Verify UI updates (highlighting, icons, counts)

---

## Troubleshooting

### Issue: Hotbar slots not showing
- **Solution:** Ensure the Hotbar panel has a Layout Group configured
- Check that Slot panels are children of the Hotbar panel
- Verify Image components are assigned to each slot's background

### Issue: Icons not displaying
- **Solution:** Ensure Item assets have an `icon` sprite assigned
- Check that Icon Image components have "Simple" type selected
- Verify Image component is not disabled

### Issue: Text not visible
- **Solution:** Change TextMeshPro text color to white or bright color
- Increase font size in TextMeshPro settings
- Ensure text is positioned on top of background (higher Z order)

### Issue: Keys not working
- **Solution:** Ensure "Allow Number Key Selection" is checked in Inspector
- Verify Input System is properly configured
- Check that the game window is focused when testing

### Issue: NullReferenceException
- **Solution:** Make sure all UI references are assigned in HotbarUIManager
- Run PopulateHotbarSlots() in Start() to auto-populate
- Check that items have valid icons assigned

---

## Complete Example Setup Checklist

- [ ] Canvas created with Canvas Scaler
- [ ] InventoryDisplay text created
- [ ] Hotbar panel created with Grid Layout Group
- [ ] 9 slot panels created inside Hotbar
- [ ] Each slot has: Background (Image), Icon (Image), Count (TMP_Text)
- [ ] Player GameObject has InventorySystem component
- [ ] Player GameObject has HotbarUIManager component
- [ ] HotbarUIManager references assigned
- [ ] Sample items created with icons
- [ ] Items added to inventory
- [ ] Test play and verify functionality

---

## Optional: Animated Slot Selection

Add this to HotbarUIManager for a smooth selection effect:

```csharp
private void OnSlotChanged(int newSlot)
{
    // Scale animation
    StartCoroutine(ScaleSlot(newSlot));
}

private System.Collections.IEnumerator ScaleSlot(int slotIndex)
{
    Transform slot = hotbarContainer.GetChild(slotIndex);
    Vector3 originalScale = slot.localScale;
    Vector3 targetScale = originalScale * 1.1f;
    
    float elapsed = 0;
    float duration = 0.1f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        slot.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
        yield return null;
    }
    
    elapsed = 0;
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        slot.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
        yield return null;
    }
}
```

---

## Summary

The hotbar UI is now integrated with your inventory system. The system handles:
- ✓ Hotbar slot selection (mouse scroll + number keys)
- ✓ Visual feedback (highlighting selected slot)
- ✓ Item display (icons and counts)
- ✓ Event system for custom effects

All NullReferenceExceptions have been fixed with proper initialization and null checks.
