using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class Slot
{
    public Item item;
    public int amount;
    public int runtimeAmount; //If item needs runtime data (e.g. water can, gun mag)

    public bool IsEmpty()
    {
        return item == null || amount <= 0;
    }

    public void Clear()
    {
        item = null;
        amount = 0;
        runtimeAmount = 0;
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

    //Shop sell slot will behave like a normal slot but not in inventory (will be accessed by index 10)
    public Slot shopSellSlot = new Slot();


    //Detect scrolling
    InputAction scrollAction;
    InputAction dropItemAction;

    //UI controller for hotbar
    public HotbarUI hotbar;

    public UnityEvent InventoryChangeEvent;

    private PlayerAudio playerAudio;

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

        dropItemAction = InputSystem.actions.FindAction("Drop Item");
        if (dropItemAction == null) Debug.LogWarning("Drop Item action not found in Input System");

        

        playerAudio = GetComponent<PlayerAudio>();
    }

    private void Start()
    {
        RefreshUI();

    }

    void Update()
    {
        if (scrollAction != null) ScrollAction();
        if (Keyboard.current != null) NumberKeySelection();
        if (dropItemAction != null) DropItemCheck();
    }

    private void DropItemCheck()
    {
        if(dropItemAction.WasPerformedThisFrame())
        {
            Item currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                int runtimeData = slots[currentSlotIndex].runtimeAmount;

                ItemDropFactory.Instance.PlayerDropItem(currentItem, runtimeData, transform.position);
                SubtractItem();
            }
        }
    }

    public void DropItemSlot(int slotIndex)
    {
        if(slotIndex < 0 || slotIndex >= slotCount) return;

        Slot slot = slots[slotIndex];
        if (slot.IsEmpty()) return;

        for(int i = 0; i < slot.amount; i++)
        {
            ItemDropFactory.Instance.PlayerDropItem(slot.item, slot.runtimeAmount, transform.position);
        }
        slot.Clear();
        RefreshUI();
    }

    //Will either combine items if same type, swap if different, or move if one is empty
    public void CombineItems(int fromInd, int toInd)
    {
        if(fromInd < 0 || fromInd > slotCount) return;
        if(toInd < 0 || toInd > slotCount) return;
        if(fromInd == toInd) return;

        Slot from, to;

        if(fromInd == 10) from = shopSellSlot;
        else from = slots[fromInd];

        if(toInd == 10) to = shopSellSlot;
        else to = slots[toInd];


        if (from.IsEmpty()) return;

        //If to slot is empty, move item
        if (to.IsEmpty())
        {
            to.item = from.item;
            to.amount = from.amount;
            to.runtimeAmount = from.runtimeAmount;
            from.Clear();
            RefreshUI();
            return;
        }

        //If same item type and stackable, combine stacks
        if (from.item == to.item && from.item.isStackable)
        {
            int totalAmount = from.amount + to.amount;
            if (totalAmount <= maxStackSize)
            {
                to.amount = totalAmount;
                from.Clear();
            }
            else
            {
                to.amount = maxStackSize;
                from.amount = totalAmount - maxStackSize;
            }
            RefreshUI();
            return;
        }

        //If different items, swap them
        Item tempItem = to.item;
        int tempAmount = to.amount;
        int tempRuntime = to.runtimeAmount;

        to.item = from.item;
        to.amount = from.amount;
        to.runtimeAmount = from.runtimeAmount;

        from.item = tempItem;
        from.amount = tempAmount;
        from.runtimeAmount = tempRuntime;

        RefreshUI();
    }


    private void ScrollAction()
    {
        bool updated = false;
        //Checks scroll wheel input
        Vector2 scrollInput = scrollAction.ReadValue<Vector2>();
        if (scrollInput.y > 0.01f)
        {
            currentSlotIndex = (currentSlotIndex - 1 + slotCount) % slotCount;
            updated = true;
        }
        else if (scrollInput.y < -0.01f)
        {
            currentSlotIndex = (currentSlotIndex + 1) % slotCount;
            updated = true;
        }
        if(updated) RefreshUI();
    }

    private void NumberKeySelection()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectSlotIndex(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectSlotIndex(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectSlotIndex(2);
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectSlotIndex(3);
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectSlotIndex(4);
        else if (Keyboard.current.digit6Key.wasPressedThisFrame) SelectSlotIndex(5);
        else if (Keyboard.current.digit7Key.wasPressedThisFrame) SelectSlotIndex(6);
        else if (Keyboard.current.digit8Key.wasPressedThisFrame) SelectSlotIndex(7);
        else if (Keyboard.current.digit9Key.wasPressedThisFrame) SelectSlotIndex(8);
        else if (Keyboard.current.digit0Key.wasPressedThisFrame) SelectSlotIndex(9);
    }

    public Item GetCurrentItem()
    {
        return slots[currentSlotIndex].item;
    }

    public int GetCurrentItemCount()
    {
        var slot = slots[currentSlotIndex];
        if(slot.item == null) return 0;

        if(slot.item.itemType == ItemType.WaterCan) return slot.runtimeAmount;
        return slot.amount;
    }

    public void ReturnShopSlotToInv()
    {
        if(shopSellSlot.IsEmpty()) return;

        //Try adding all items in slot back to inventory
        for(int i = 0; i < shopSellSlot.amount; i++)
        {
            AddItem(shopSellSlot.item, shopSellSlot.runtimeAmount);
        }

        shopSellSlot.Clear();
        RefreshUI();
    }

    //Call when item is picked up
    public void AddItem(Item collectedItem, int runtimeData)
    {
        //Try adding item to inventory
        if (!TryToAddItem(collectedItem, runtimeData))
        {
            //If inventory full, re-drop item at player's position
            ItemDropFactory.Instance.PlayerDropItem(collectedItem, runtimeData, transform.position);
        }

        playerAudio.PlayInventoryPop();
    }

    private bool TryToAddItem(Item newItem, int runtimeData)
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
                    slot.runtimeAmount = runtimeData;
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
            slots[currentSlotIndex].runtimeAmount = waterData.maxWater;
        }
        RefreshUI();
    }

    //Use water from watering can
    public void UseWater()
    {
        if (slots[currentSlotIndex].runtimeAmount > 0)
        {
            slots[currentSlotIndex].runtimeAmount--;
        }
        RefreshUI();
    }


    //Refreshes the hotbar UI
    public void RefreshUI()
    {
        hotbar.UpdateUI(slots, shopSellSlot, currentSlotIndex);
        InventoryChangeEvent.Invoke();
    }

    public Slot GetSellSlot()
    {
        return shopSellSlot;
    }

    public void SubtractSellSlot()
    {
        shopSellSlot.amount--;
        if(shopSellSlot.amount <= 0)
        {
            shopSellSlot.Clear();
        }
        RefreshUI();
    }

    public void SellSlotClick()
    {
        if (shopSellSlot.IsEmpty()) CombineItems(currentSlotIndex, 10);
        else ReturnShopSlotToInv();
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
                if (slots[i].runtimeAmount > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
