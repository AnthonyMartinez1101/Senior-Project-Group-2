# Hotbar UI Not Showing - Troubleshooting Guide

## Quick Diagnosis

If your hotbar UI is not showing sprites or selection highlights, follow these steps:

### Step 1: Run the Diagnostic

1. **Attach HotbarDiagnostic.cs** to any GameObject in your scene
2. **Play the scene**
3. **Look for the diagnostic button** in the top-left corner of the game view
4. **Click it** and check the Console output

The diagnostic will tell you exactly what's wrong.

---

## Common Issues & Fixes

### Issue 1: "No valid hotbar slots found"

**Cause:** The UI hierarchy doesn't match what the system expects.

**Fix:**
1. In your Canvas, make sure you have a **Hotbar** panel
2. Each slot should look like this:
   ```
   Slot_1 (Panel with Image component)
   ├── Icon (Image)
   └── Count (TextMeshPro Text)
   ```

3. **Key point:** The slot panel itself MUST have an Image component (this is the background)

---

### Issue 2: Sprites appear, but selection highlight isn't working

**Cause:** Hotbar container isn't being found, so slots aren't linked.

**Possible fixes:**
- Your hotbar panel might have a different name (not exactly "Hotbar")
- Check the Console during startup for messages like:
  ```
  "Could not find Hotbar container in Canvas"
  ```

**Solution:**
- Option A: Rename your hotbar panel to exactly "Hotbar"
- Option B: Manually assign the Hotbar container in HotbarUIManager inspector

---

### Issue 3: Slots show but weapon sprite not appearing

**Cause:** The weapon item doesn't have an icon sprite assigned.

**Fix:**
1. Find your Pistol Item ScriptableObject
2. In the Inspector, drag a sprite into the **icon** field
3. Make sure it's not a null or missing reference
4. Check Console for: `"NO ICON"` warnings

---

### Issue 4: Selection highlight changes but UI doesn't update

**Cause:** `UpdateDisplayText()` isn't being called, or UI elements aren't linked.

**Manual Fix:**
1. Attach `HotbarDiagnostic.cs` to a GameObject
2. Call `ManualRefreshHotbarUI()` from it
3. If this makes it work, the issue is with automatic updates

---

## Step-by-Step Manual Setup

If auto-discovery isn't working, try this manual approach:

### 1. Create a Manager Script

Create a new script called `ManualHotbarSetup.cs`:

```csharp
using UnityEngine;

public class ManualHotbarSetup : MonoBehaviour
{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Transform hotbarContainer;
    
    private void Start()
    {
        if (inventorySystem == null)
            inventorySystem = GetComponent<InventorySystem>();
        
        ManuallyLinkSlots();
    }
    
    private void ManuallyLinkSlots()
    {
        inventorySystem.ClearHotbarSlots();
        
        // Manually link each slot
        for (int i = 0; i < hotbarContainer.childCount && i < 9; i++)
        {
            Transform slot = hotbarContainer.GetChild(i);
            
            Image background = slot.GetComponent<Image>();
            Image icon = slot.Find("Icon")?.GetComponent<Image>();
            TMP_Text count = slot.Find("Count")?.GetComponent<TMP_Text>();
            
            if (background != null && icon != null && count != null)
            {
                inventorySystem.AddHotbarSlot(background, count, icon);
                Debug.Log($"Manually linked slot {i}");
            }
            else
            {
                Debug.LogWarning($"Slot {i} is missing components!");
            }
        }
        
        inventorySystem.UpdateDisplayText();
    }
}
```

### 2. Assign References
1. Create a new empty GameObject called "HotbarManager"
2. Add the `ManualHotbarSetup.cs` script
3. In the Inspector:
   - **InventorySystem**: Drag your Player object
   - **Hotbar Container**: Drag your Hotbar panel

---

## Debugging Checklist

Use this checklist to verify your setup:

- [ ] Canvas exists in the scene
- [ ] Canvas has a child panel called "Hotbar"
- [ ] Hotbar has 9 child panels named "Slot_1" through "Slot_9"
- [ ] Each Slot has an **Image component** (this is the background)
- [ ] Each Slot has a child named "Icon" with Image component
- [ ] Each Slot has a child named "Count" with TextMeshPro Text
- [ ] Player has InventorySystem component
- [ ] Player has HotbarUIManager component
- [ ] HotbarUIManager can find the InventorySystem (check OnEnable)
- [ ] Pistol item has an icon sprite assigned
- [ ] Pistol is in the inventory (check with diagnostic)

---

## Console Output Interpretation

### Good Output:
```
HotbarUIManager: Found 9 potential slot objects
✓ Linked Hotbar Slot 1: 'Slot_1'
✓ Linked Hotbar Slot 2: 'Slot_2'
...
HotbarUIManager: Successfully linked 9 hotbar slots
```

### Bad Output:
```
HotbarUIManager: Could not find Hotbar container in Canvas. Please assign it manually in the Inspector.
```
**Action:** Manually assign Hotbar Container in HotbarUIManager inspector

### Warning Output:
```
✗ Slot 'Slot_1' missing components - Background: False, Icon: True, Count: True
```
**Action:** Add Image component to Slot_1 panel itself

---

## If Nothing Works

Try this full reset:

1. **Delete your old Hotbar** from the Canvas
2. **Create a new one** following the exact structure:
   ```
   Canvas
   └── Hotbar (Panel)
       ├── LayoutGroup: Grid Layout Group
       └── 9x Slot panels, each with:
           - Image component
           - Grid Layout Child component
           - Child named "Icon" (Image)
           - Child named "Count" (TextMeshPro)
   ```

3. **Reassign in Inspector:**
   - HotbarUIManager → Hotbar Container: Set to your new Hotbar
   - InventorySystem → Colors and settings

4. **Play scene** - it should auto-discover now

---

## Performance Tips

- Use the `HotbarDiagnostic` during development to catch issues early
- Only call `UpdateDisplayText()` when inventory changes (it already does this automatically)
- If you have many items, consider limiting `hotbarSize` to reduce UI updates

---

## Support Info

If the above doesn't work, provide this information:

1. Screenshot of your UI hierarchy
2. Console output from `HotbarDiagnostic`
3. Are the item sprites showing at all?
4. What color is the selection highlight (or is it missing)?
