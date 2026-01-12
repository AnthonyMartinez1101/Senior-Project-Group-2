//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//#if UNITY_EDITOR
//using UnityEditor;
//#endif

///// <summary>
///// Quick diagnostic tool to help troubleshoot hotbar UI issues
///// Attach this to any GameObject and use the public methods via Inspector buttons or console
///// </summary>
//public class HotbarDiagnostic : MonoBehaviour
//{
//    [SerializeField] private InventorySystem inventorySystem;
//    [SerializeField] private Canvas uiCanvas;
    
//    private void OnGUI()
//    {
//        if (GUILayout.Button("Run Full Hotbar Diagnostic", GUILayout.Height(40)))
//        {
//            RunDiagnostic();
//        }
//    }
    
//    public void RunDiagnostic()
//    {
//        Debug.Log("\n╔════════════════════════════════════════════════════════╗");
//        Debug.Log("║          HOTBAR DIAGNOSTIC - STARTING                 ║");
//        Debug.Log("╚════════════════════════════════════════════════════════╝");
        
//        // Find components if not assigned
//        if (inventorySystem == null)
//            inventorySystem = FindFirstObjectByType<InventorySystem>();
//        if (uiCanvas == null)
//            uiCanvas = FindFirstObjectByType<Canvas>();
        
//        if (inventorySystem == null)
//        {
//            Debug.LogError("❌ ERROR: InventorySystem not found in scene!");
//            return;
//        }
        
//        Debug.Log("\n📋 INVENTORY SYSTEM STATUS");
//        Debug.Log($"   ✓ Found: {inventorySystem.gameObject.name}");
//        Debug.Log($"   • Items in inventory: {inventorySystem.inventoryItems.Count}");
//        Debug.Log($"   • Current selected slot: {inventorySystem.GetCurrentHotbarSlot()}");
//        Debug.Log($"   • Hotbar size setting: {inventorySystem.hotbarSize}");
        
//        Debug.Log("\n🎯 INVENTORY CONTENTS");
//        for (int i = 0; i < inventorySystem.inventoryItems.Count; i++)
//        {
//            var stack = inventorySystem.inventoryItems[i];
//            string selection = i == inventorySystem.GetCurrentHotbarSlot() ? "  >>> SELECTED <<<" : "";
//            if (stack.item != null)
//            {
//                string icon = stack.item.icon != null ? "✓ YES" : "❌ MISSING";
//                Debug.Log($"   [{i}] {stack.item.itemName.PadRight(20)} x{stack.count}  Icon: {icon}{selection}");
//            }
//            else
//            {
//                Debug.Log($"   [{i}] (empty){selection}");
//            }
//        }
        
//        if (uiCanvas == null)
//        {
//            Debug.LogWarning("⚠️ WARNING: No Canvas found in scene!");
//        }
//        else
//        {
//            Debug.Log("\n🖼️  UI CANVAS STATUS");
//            Debug.Log($"   ✓ Found: {uiCanvas.gameObject.name}");
            
//            Transform hotbarTransform = uiCanvas.transform.Find("Hotbar");
//            if (hotbarTransform != null)
//            {
//                Debug.Log($"   ✓ Hotbar panel found: {hotbarTransform.name}");
//                Debug.Log($"   • Slot objects: {hotbarTransform.childCount}");
                
//                int validSlots = 0;
//                for (int i = 0; i < hotbarTransform.childCount && i < 9; i++)
//                {
//                    Transform slot = hotbarTransform.GetChild(i);
//                    Image background = slot.GetComponent<Image>();
//                    Image icon = slot.Find("Icon")?.GetComponent<Image>();
//                    TMP_Text count = slot.Find("Count")?.GetComponent<TMP_Text>();
                    
//                    bool isValid = background != null && icon != null && count != null;
//                    string status = isValid ? "✓" : "❌";
                    
//                    Debug.Log($"      {status} Slot {i}: '{slot.name}' - BG:{background != null} Icon:{icon != null} Count:{count != null}");
                    
//                    if (isValid) validSlots++;
//                }
                
//                Debug.Log($"\n   • Valid slots linked: {validSlots}/9");
                
//                if (validSlots == 0)
//                {
//                    Debug.LogError("\n❌ CRITICAL: No valid hotbar slots detected!");
//                    Debug.Log("   FIX: Ensure each slot has:");
//                    Debug.Log("        1. Image component on the slot panel (background)");
//                    Debug.Log("        2. Child object named 'Icon' with Image component");
//                    Debug.Log("        3. Child object named 'Count' with TextMeshPro component");
//                }
//            }
//            else
//            {
//                Debug.LogError("❌ ERROR: 'Hotbar' panel not found under Canvas!");
//                Debug.Log("   Found children of Canvas:");
//                foreach (Transform child in uiCanvas.transform)
//                {
//                    Debug.Log($"      - {child.name}");
//                }
//            }
//        }
        
//        Debug.Log("\n" + new string('═', 56));
//        Debug.Log("                   DIAGNOSTIC COMPLETE");
//        Debug.Log(new string('═', 56) + "\n");
//    }
    
//    /// <summary>
//    /// Manually trigger UpdateDisplayText to refresh the hotbar UI
//    /// </summary>
//    public void ManualRefreshHotbarUI()
//    {
//        if (inventorySystem != null)
//        {
//            Debug.Log("🔄 Manually refreshing hotbar UI...");
//            inventorySystem.UpdateDisplayText();
//            Debug.Log("✓ Refresh complete");
//        }
//    }
    
//    /// <summary>
//    /// Print all item icons and their status
//    /// </summary>
//    public void CheckAllItemIcons()
//    {
//        Debug.Log("\n📸 ITEM ICON CHECK");
//        for (int i = 0; i < inventorySystem.inventoryItems.Count; i++)
//        {
//            var item = inventorySystem.inventoryItems[i].item;
//            if (item != null)
//            {
//                string iconStatus = item.icon != null ? "✓ HAS ICON" : "❌ NO ICON";
//                Debug.Log($"   [{i}] {item.itemName}: {iconStatus}");
//            }
//        }
//    }
//}
