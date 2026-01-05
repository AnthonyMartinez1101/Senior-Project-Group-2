using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class Slot
{
    public Item item;
    public int amount;
    public int waterAmount; //if item is water bucket

    public bool IsEmpty()
    {
        return item == null || amount <= 0;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
        waterAmount = 0;
    }
}

public class Inventory : MonoBehaviour
{
    //Inventory settings
    public int maxStackSize = 32;
    private const int slotCount = 10;

    //Current selected slot
    private int currentSlotIndex = 0;

    //Creating the actual array of slots to be the inventory
    public List<Slot> slots = new List<Slot>(slotCount);

    //Detect scrolling
    InputAction scrollAction;

    //UI controller for hotbar
    public HotbarUI hotbar;


    private void Awake()
    {
        //Creates and assigns the list of slots
        for (int i = 0; i < slotCount; i++)
        {
            slots.Add(new Slot());
        }
        
        //Set scroll action
        scrollAction = InputSystem.actions.FindAction("Scroll");
        if(scrollAction == null) Debug.LogWarning("Scroll action not found in Input System");

        RefreshUI();
    }

    void Update()
    {
        if (scrollAction != null)
        {
            //Checks scroll wheel input
            Vector2 scrollInput = scrollAction.ReadValue<Vector2>();
            if (scrollInput.y > 0.01f) currentSlotIndex = (currentSlotIndex - 1 + slotCount) % slotCount;
            else if (scrollInput.y < -0.01f) currentSlotIndex = (currentSlotIndex + 1) % slotCount;
            RefreshUI();
        }
    }

    public Item GetCurrentItem()
    {
        return slots[currentSlotIndex].item;
    }

    public int GetCurrentItemCount()
    {
        var slot = slots[currentSlotIndex];
        if(slot.item == null) return 0;

        if(slot.item.itemType == ItemType.WaterCan) return slot.waterAmount;
        return slot.amount;
    }

    //Call when item is picked up
    public void AddItem(Item collectedItem)
    {
        //Try adding item to inventory
        if (!TryToAddItem(collectedItem))
        {
            //If inventory full, re-drop item at player's position
            ItemDropFactory.Instance.PlayerDropItem(collectedItem, transform.position);
        }
    }

    private bool TryToAddItem(Item newItem)
    {
        bool added = false;
        int changedIndex = -1;

        // Check for stackable items first
        if (newItem.isStackable)
        {
            //Iterate through each slot
            foreach(Slot slot in slots) 
            {
                //Find first slot with same item that is not max
                if(!slot.IsEmpty() && slot.item == newItem && slot.amount < maxStackSize)
                {
                    slot.amount++;
                    added = true;
                    changedIndex = slots.IndexOf(slot);
                    break;
                }
            }
        }

        // If not stackable or no existing stack found, find an empty slot
        if(!added)
        {
            //Iterate through each slot
            foreach (Slot slot in slots) 
            {
                //Find and add to first empty slot
                if (slot.IsEmpty())
                {
                    slot.item = newItem;
                    slot.amount = 1;
                    changedIndex = slots.IndexOf(slot);
                    added = true;
                    break;
                }
            }
        }
        RefreshUI();

        if(added && changedIndex != -1)
        {
            hotbar.BobIcon(changedIndex);
        }

        return added;  
    }

    //Remove item and clear if amount reaches 0
    public void SubtractItem()
    {
        slots[currentSlotIndex].amount--;
        if (slots[currentSlotIndex].amount <= 0)
        {
            slots[currentSlotIndex].Clear();
        }
        RefreshUI();
    }

    public bool CheckAndUseBullets()
    {
        for(int i = 0; i < slotCount; i++)
        {
            if(!slots[i].IsEmpty() && slots[i].item.itemType == ItemType.Bullet)
            {
                slots[i].amount--;
                if (slots[i].amount <= 0)
                {
                    slots[i].Clear();
                }
                RefreshUI();
                return true;
            }
        }
        return false;
    }

    //Refill water in watering can
    public void RefillWater()
    {
        var waterData = slots[currentSlotIndex].item.extraItemData as BucketData;
        if(waterData != null)
        {
            slots[currentSlotIndex].waterAmount = waterData.maxWater;
        }
        RefreshUI();
    }

    //Use water from watering can
    public void UseWater()
    {
        if (slots[currentSlotIndex].waterAmount > 0)
        {
            slots[currentSlotIndex].waterAmount--;
        }
        RefreshUI();
    }


    //Refreshes the hotbar UI
    public void RefreshUI()
    {
        hotbar.UpdateUI(slots, currentSlotIndex);
    }

    //Selects a new slot by index
    public void SelectSlotIndex(int newIndex)
    {
        if (newIndex >= 0 && newIndex < slotCount)
        {
            currentSlotIndex = newIndex;
            RefreshUI();
        }
    }

    // Return true if item is found in inventory
    public int SearchForItem(Item itemToFind)
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (!slots[i].IsEmpty() && slots[i].item == itemToFind)
            {
                return slots[i].amount;
            }
        }
        return 0;
    }

    public bool IsWaterBucketEmpty()
    {
        for(int i = 0; i < slotCount; i++)
        {
            if (!slots[i].IsEmpty() && slots[i].item.itemType == ItemType.WaterCan)
            {
                if (slots[i].waterAmount > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
