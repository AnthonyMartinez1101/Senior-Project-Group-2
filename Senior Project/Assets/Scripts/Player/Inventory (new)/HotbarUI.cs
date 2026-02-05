using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    private List<SlotUI> slotUIs = new List<SlotUI>(10);
    [SerializeField] private SlotUI sellSlotUI;

    void Awake()
    {
        slotUIs.Clear();

        var slotUIScripts = GetComponentsInChildren<SlotUI>();

        foreach (var slotUIScript in slotUIScripts)
        {
            slotUIs.Add(slotUIScript);
        }
    }

    public void BobIcon(int index)
    {
        if(index < 0 || index >= slotUIs.Count) return;
        slotUIs[index].Bob();
    }

    public void UpdateUI(List<Slot> slots, Slot sellSlot, int current)
    {
        //For each slot UI...
        for (int i = 0; i < slotUIs.Count; i++)
        {
            UpdateSlot(slots[i], slotUIs[i], i, current);
        }

        //Update sell slot UI
        UpdateSlot(sellSlot, sellSlotUI, -1, -2);
    }

    private void UpdateSlot(Slot slot, SlotUI slotUI, int i, int currentlySelected)
    {
        //Highlight slot if currently selected
        if (i == currentlySelected) slotUI.background.color = Color.yellow;
        else slotUI.background.color = Color.gray;

        //Resets UI first
        slotUI.itemIcon.sprite = null;
        slotUI.itemIcon.enabled = false;
        slotUI.itemAmountText.text = "";

        //Hide water meter UI by default
        if (slotUI.waterMeter != null) slotUI.waterMeter.gameObject.SetActive(false);

        //If slot is empty, continue to next iteration
        if (slot.item == null) return;

        //If slot has item, set item icon
        slotUI.itemIcon.enabled = true;

        //If item is not water can, set normal item icon
        if (slot.item.itemType != ItemType.WaterCan)
        {
            slotUI.itemIcon.sprite = slot.item.icon;
        }

        //If item is water can...
        else
        {
            //...get bucket info
            var bucketData = slot.item.extraItemData as BucketData;

            //If bucket data exists...
            if (bucketData != null)
            {
                //...set icon based on water amount
                if (slot.runtimeAmount > 0) slotUI.itemIcon.sprite = bucketData.fullSprite;
                else slotUI.itemIcon.sprite = bucketData.emptySprite;

                //...enable and update water meter
                if (slotUI.waterMeter != null)
                {
                    slotUI.waterMeter.gameObject.SetActive(true);
                    slotUI.waterMeter.SetMaxWater(bucketData.maxWater);
                    slotUI.waterMeter.SetWater(slot.runtimeAmount);
                }
            }
            //...if no bucket data, just set normal item icon
            else
            {
                slotUI.itemIcon.sprite = slot.item.icon;
            }
        }

        //Stack count text (only for stackable, non-watercan items)
        if (slot.item.itemType != ItemType.WaterCan)
        {
            if (slot.item.isStackable)
            {
                if (slot.amount > 1)
                {
                    slotUI.itemAmountText.text = slot.amount.ToString();
                }
                else
                {
                    slotUI.itemAmountText.text = "";
                }
            }
            else
            {
                slotUI.itemAmountText.text = "";
            }
        }
    }
}
