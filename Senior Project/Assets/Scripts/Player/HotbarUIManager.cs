using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages the UI display and interaction for the hotbar system.
/// Links the InventorySystem with UI elements and provides visual feedback.
/// </summary>
public class HotbarUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Transform hotbarContainer;  // The Hotbar panel
    
    [Header("Visual Settings")]
    [SerializeField] private bool useSlotAnimations = true;
    [SerializeField] private float selectionAnimationDuration = 0.1f;
    [SerializeField] private float selectionScale = 1.1f;
    
    private void OnEnable()
    {
        if (inventorySystem != null)
        {
            // Subscribe to inventory events
            inventorySystem.OnHotbarSlotChanged.AddListener(OnSlotChanged);
            inventorySystem.OnHeldItemChanged.AddListener(OnItemChanged);
        }
    }

    private void OnDisable()
    {
        if (inventorySystem != null)
        {
            // Unsubscribe from events
            inventorySystem.OnHotbarSlotChanged.RemoveListener(OnSlotChanged);
            inventorySystem.OnHeldItemChanged.RemoveListener(OnItemChanged);
        }
    }

    private void Start()
    {
        // Auto-find InventorySystem if not assigned
        if (inventorySystem == null)
        {
            inventorySystem = GetComponent<InventorySystem>();
            if (inventorySystem == null)
            {
                inventorySystem = FindFirstObjectByType<InventorySystem>();
            }
        }
        
        if (inventorySystem == null)
        {
            Debug.LogError("HotbarUIManager: InventorySystem not found!");
            return;
        }
        
        // Auto-find Hotbar container if not assigned
        if (hotbarContainer == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                hotbarContainer = canvas.transform.Find("Hotbar");
                if (hotbarContainer == null)
                {
                    // Try alternative names
                    hotbarContainer = canvas.transform.Find("HotbarPanel");
                    if (hotbarContainer == null)
                        hotbarContainer = canvas.transform.Find("Inventory Hotbar");
                }
            }
        }
        
        // Populate hotbar slots from UI hierarchy
        if (hotbarContainer != null)
        {
            PopulateHotbarSlots();
        }
        else
        {
            Debug.LogError("HotbarUIManager: Could not find Hotbar container in Canvas. Please assign it manually in the Inspector.");
        }
    }
    
    /// <summary>
    /// Automatically discovers and links UI slot elements from the hierarchy.
    /// </summary>
    private void PopulateHotbarSlots()
    {
        if (hotbarContainer == null)
        {
            //Debug.LogError("HotbarUIManager: Hotbar container is null!");
            return;
        }

        // Clear existing references
        inventorySystem.ClearHotbarSlots();
        
        int slotIndex = 0;
        int successfulSlots = 0;
        
        //Debug.Log($"HotbarUIManager: Found {hotbarContainer.childCount} potential slot objects");
        
        // Iterate through each child of the hotbar container
        foreach (Transform slotTransform in hotbarContainer)
        {
            if (slotIndex >= inventorySystem.hotbarSize)
                break;
            
            // Get the background image (on the slot panel itself)
            Image background = slotTransform.GetComponent<Image>();
            
            // Find child elements
            Image icon = null;
            TMP_Text count = null;
            
            // Search for Icon and Count components in children
            foreach (Transform child in slotTransform)
            {
                if (icon == null && (child.name.Contains("Icon") || child.name.Contains("icon")))
                    icon = child.GetComponent<Image>();
                
                if (count == null && (child.name.Contains("Count") || child.name.Contains("count") || child.name.Contains("Text")))
                    count = child.GetComponent<TMP_Text>();
            }
            
            // Only add slot if all components are found
            if (background != null && icon != null && count != null)
            {
                inventorySystem.AddHotbarSlot(background, count, icon);
                successfulSlots++;
                //Debug.Log($"✓ Linked Hotbar Slot {slotIndex + 1}: '{slotTransform.name}'");
                slotIndex++;
            }
            else
            {
                //Debug.LogWarning($"✗ Slot '{slotTransform.name}' missing components - Background: {background != null}, Icon: {icon != null}, Count: {count != null}");
            }
        }
        
        if (successfulSlots == 0)
        {
            //Debug.LogError("HotbarUIManager: No valid hotbar slots found! Check your UI hierarchy structure.");
            //Debug.Log("Expected structure:\n  Canvas\n    └─ Hotbar (or similar name)\n        ├─ Slot_1 (with Image component)\n        │   ├─ Icon (Image)\n        │   └─ Count (TextMeshPro)\n        ├─ Slot_2\n        ...");
            return;
        }
        
        //Debug.Log($"HotbarUIManager: Successfully linked {successfulSlots} hotbar slots");
        inventorySystem.UpdateDisplayText();
    }
    
    /// <summary>
    /// Called when the hotbar slot selection changes
    /// </summary>
    private void OnSlotChanged(int newSlot)
    {
        //Debug.Log($"Hotbar Slot Changed: {newSlot + 1}");
        
        if (useSlotAnimations && hotbarContainer != null && newSlot >= 0 && newSlot < hotbarContainer.childCount)
        {
            Transform slotTransform = hotbarContainer.GetChild(newSlot);
            if (slotTransform != null)
            {
                StartCoroutine(PlaySlotSelectionAnimation(slotTransform));
            }
        }
    }
    
    /// <summary>
    /// Called when the held item changes
    /// </summary>
    private void OnItemChanged(ItemStack newItem)
    {
        if (newItem != null && newItem.item != null)
        {
            //Debug.Log($"Item Changed: {newItem.item.itemName} (Count: {newItem.count})");
        }
    }
    
    /// <summary>
    /// Plays a scale animation when a slot is selected
    /// </summary>
    private IEnumerator PlaySlotSelectionAnimation(Transform slotTransform)
    {
        Vector3 originalScale = slotTransform.localScale;
        Vector3 targetScale = originalScale * selectionScale;
        
        float elapsed = 0;
        
        // Scale up
        while (elapsed < selectionAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / selectionAnimationDuration;
            slotTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        elapsed = 0;
        
        // Scale back down
        while (elapsed < selectionAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / selectionAnimationDuration;
            slotTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
        
        slotTransform.localScale = originalScale;
    }
    
    /// <summary>
    /// Manual method to refresh all hotbar slots from UI
    /// Useful if you dynamically add/remove slots at runtime
    /// </summary>
    public void RefreshHotbarSlots()
    {
        PopulateHotbarSlots();
    }
    
    /// <summary>
    /// Gets the current selected slot index
    /// </summary>
    public int GetCurrentSlot()
    {
        return inventorySystem.GetCurrentHotbarSlot();
    }
    
    /// <summary>
    /// Manually select a hotbar slot by index (0-based)
    /// </summary>
    public void SelectSlot(int slotIndex)
    {
        inventorySystem.SetHotbarSlot(slotIndex);
    }
    
    /// <summary>
    /// Debug method to verify hotbar setup and connections
    /// Call this from console or a button to diagnose issues
    /// </summary>
    public void DebugHotbarSetup()
    {
        //Debug.Log("\n========== HOTBAR DEBUG INFO ==========");
        
        if (inventorySystem == null)
        {
            //Debug.LogError("❌ InventorySystem is NULL");
            return;
        }
        
        //Debug.Log($"✓ InventorySystem found");
        //Debug.Log($"  - Inventory Items: {inventorySystem.inventoryItems.Count}");
        //Debug.Log($"  - Current Index: {inventorySystem.GetCurrentHotbarSlot()}");
        //Debug.Log($"  - Hotbar Size Setting: {inventorySystem.hotbarSize}");
        
        if (hotbarContainer == null)
        {
            //Debug.LogError("❌ Hotbar Container is NULL - UI not connected!");
            return;
        }
        
        //Debug.Log($"✓ Hotbar Container found: '{hotbarContainer.name}'");
        
        // Check if slot lists are populated
        var slotBackgroundsField = typeof(InventorySystem).GetField("slotBackgrounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var slotIconsField = typeof(InventorySystem).GetField("slotIcons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var slotCountsField = typeof(InventorySystem).GetField("slotCounts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (slotBackgroundsField != null)
        {
            var backgrounds = slotBackgroundsField.GetValue(inventorySystem) as List<Image>;
            var icons = slotIconsField.GetValue(inventorySystem) as List<Image>;
            var counts = slotCountsField.GetValue(inventorySystem) as List<TMP_Text>;
            
            //Debug.Log($"  - Slot Backgrounds Linked: {backgrounds?.Count ?? 0}");
            //Debug.Log($"  - Slot Icons Linked: {icons?.Count ?? 0}");
            //Debug.Log($"  - Slot Counts Linked: {counts?.Count ?? 0}");
            
            //if (backgrounds?.Count == 0)
                //Debug.LogWarning("⚠️ WARNING: No slots are linked to the InventorySystem!");
        }
        
        //Debug.Log("\n========== INVENTORY CONTENTS ==========");
        for (int i = 0; i < inventorySystem.inventoryItems.Count; i++)
        {
            var stack = inventorySystem.inventoryItems[i];
            if (stack.item != null)
            {
                bool hasIcon = stack.item.icon != null;
                string marker = i == inventorySystem.GetCurrentHotbarSlot() ? ">>>" : "   ";
                //Debug.Log($"{marker} Slot {i}: {stack.item.itemName} x{stack.count} {(hasIcon ? "✓ Has Icon" : "❌ NO ICON")}");
            }
            else
            {
                //Debug.Log($"    Slot {i}: (empty)");
            }
        }
        
        //Debug.Log("========== END DEBUG ==========\n");
    }
}
