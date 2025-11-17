# Consumable vs Non-Consumable Items Guide

## Overview

The inventory system now supports two types of items:

1. **Consumable Items** (disappear when depleted)
   - Seeds
   - Produce
   - Weapons with limited ammo
   
2. **Non-Consumable Items** (stay in inventory, resource depletes)
   - Watering Can (holds water)
   - Tools that can be refilled
   - Containers with resources

---

## How to Set Up Items

### Step 1: Mark Items as Consumable or Non-Consumable

1. Select your **Watering Can** item in the Inspector
2. Find the **ITEM BEHAVIOR** section
3. **Uncheck** "Is Consumable" 
4. Save the asset

For your **Seeds**:
1. Select a **Seed** item in the Inspector
2. Find the **ITEM BEHAVIOR** section
3. **Check** "Is Consumable" (should be default)
4. Save the asset

### Step 2: Behavior Differences

**Consumable Item (checked):**
- When count reaches 0, the item disappears from inventory
- Example: You plant 5 seeds, they all become 0, seed item is removed

**Non-Consumable Item (unchecked):**
- When count reaches 0, the item stays in inventory
- Count just shows 0
- Can be refilled with water/resources
- Example: Watering can has 0 water, but you can refill it to 5

---

## Code Usage

### Subtracting from Current Item

```csharp
// This handles consumable/non-consumable logic automatically
inventorySystem.SubtractItem();
```

### Subtracting from a Specific Slot

```csharp
// Subtract 1 water from slot 0 (doesn't delete the item)
inventorySystem.SubtractItemAtSlot(0, 1);

// Subtract 3 water
inventorySystem.SubtractItemAtSlot(0, 3);
```

### Refilling an Item

```csharp
// Add 5 water back to slot 0
inventorySystem.AddToItemAtSlot(0, 5);

// Or reset the watering can counter directly
foreach (var stack in inventorySystem.inventoryItems)
{
    if (stack.item.itemType == ItemType.WaterCan)
    {
        stack.count = 5;
        inventorySystem.UpdateDisplayText();
        break;
    }
}
```

---

## Example: Watering a Plant

```csharp
public class PlantController : MonoBehaviour
{
    public InventorySystem inventorySystem;
    
    public void WaterPlant()
    {
        Item currentItem = inventorySystem.GetCurrentItem();
        
        if (currentItem != null && currentItem.itemType == ItemType.WaterCan)
        {
            int waterCount = inventorySystem.GetCurrentItemCount();
            
            if (waterCount > 0)
            {
                // Use water
                inventorySystem.SubtractItem();
                
                // Apply water effect to plant
                Plant plant = GetComponent<Plant>();
                if (plant != null)
                    plant.AddWater();
                
                Debug.Log($"Plant watered! Water remaining: {inventorySystem.GetCurrentItemCount()}");
            }
            else
            {
                Debug.Log("Watering can is empty! Find a well to refill.");
            }
        }
    }
}
```

---

## Example: Refilling at a Well

```csharp
public class Well : MonoBehaviour
{
    public InventorySystem inventorySystem;
    
    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Item currentItem = inventorySystem.GetCurrentItem();
            
            if (currentItem != null && currentItem.itemType == ItemType.WaterCan)
            {
                inventorySystem.RefillWater();
                Debug.Log("Watering can refilled!");
            }
            else
            {
                Debug.Log("You need a watering can to refill!");
            }
        }
    }
}
```

---

## Inspector Settings Checklist

**For Watering Can:**
- [ ] Item Name: "Watering Can"
- [ ] Icon: Assigned (watering can sprite)
- [ ] Item Type: "WaterCan"
- [ ] **Is Consumable: UNCHECKED** ✓

**For Seeds:**
- [ ] Item Name: "Tomato Seed" (or crop name)
- [ ] Icon: Assigned (seed sprite)
- [ ] Item Type: "Seed"
- [ ] Plant: Assigned (tomato plant prefab)
- [ ] **Is Consumable: CHECKED** ✓

**For Produce:**
- [ ] Item Name: "Tomato"
- [ ] Icon: Assigned (tomato sprite)
- [ ] Item Type: "Produce"
- [ ] Heal Amount: 25 (or your value)
- [ ] **Is Consumable: CHECKED** ✓

---

## Debugging

To verify your items are set up correctly, check the Console output when you use SubtractItem():

```
[Hotbar >>>] Slot 0: Watering Can x5 | Icon: ✓
[Hotbar    ] Slot 1: Tomato Seed x3 | Icon: ✓
```

When you subtract from the watering can:
```
[Hotbar >>>] Slot 0: Watering Can x4 | Icon: ✓  ← Item stays, count decreased
```

When you subtract from a seed:
```
[Hotbar    ] Slot 1: Tomato Seed x2 | Icon: ✓  ← Item stays while count > 0
[Hotbar    ] Slot 1: (empty)                   ← Item removed when count reaches 0
```

---

## Summary

| Aspect | Consumable | Non-Consumable |
|--------|-----------|-----------------|
| Disappears at 0? | Yes | No |
| Use Case | Seeds, produce, ammo | Watering can, tools |
| When to use | One-time use items | Reusable containers |
| Refill? | No | Yes |
| In Inspector | Checked | Unchecked |

That's it! Your watering can will now persist in your inventory even when empty, and you can refill it at wells.
