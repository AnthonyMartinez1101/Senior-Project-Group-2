using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    private List<SlotUI> slotUIs = new List<SlotUI>(10);

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

    public void UpdateUI(List<Slot> slots, int currentlySelected)
    {
        //For each slot UI...
        for (int i = 0; i < slotUIs.Count; i++)
        {
            //Highlight slot if currently selected
            if (i == currentlySelected) slotUIs[i].background.color = Color.yellow;
            else slotUIs[i].background.color = Color.gray;

            //Resets UI first
            slotUIs[i].itemIcon.sprite = null;
            slotUIs[i].itemIcon.enabled = false;
            slotUIs[i].itemAmountText.text = "";

            //Hide water meter UI by default
            if (slotUIs[i].waterMeter != null) slotUIs[i].waterMeter.gameObject.SetActive(false);

            //If slot is empty, continue to next iteration
            if (slots[i].item == null) continue;    

            //If slot has item, set item icon
            slotUIs[i].itemIcon.enabled = true;

            //If item is not water can, set normal item icon
            if (slots[i].item.itemType != ItemType.WaterCan)
            {
                slotUIs[i].itemIcon.sprite = slots[i].item.icon;
            }

            //If item is water can...
            else
            {
                //...get bucket info
                var bucketData = slots[i].item.extraItemData as BucketData;

                //If bucket data exists...
                if (bucketData != null)
                {
                    //...set icon based on water amount
                    if (slots[i].runtimeAmount > 0) slotUIs[i].itemIcon.sprite = bucketData.fullSprite;
                    else slotUIs[i].itemIcon.sprite = bucketData.emptySprite;

                    //...enable and update water meter
                    if (slotUIs[i].waterMeter != null)
                    {
                        slotUIs[i].waterMeter.gameObject.SetActive(true);
                        slotUIs[i].waterMeter.SetMaxWater(bucketData.maxWater);
                        slotUIs[i].waterMeter.SetWater(slots[i].runtimeAmount);
                    }
                }
                //...if no bucket data, just set normal item icon
                else
                {
                    slotUIs[i].itemIcon.sprite = slots[i].item.icon;
                }
            }

            //Stack count text (only for stackable, non-watercan items)
            if (slots[i].item.itemType != ItemType.WaterCan)
            {
                if (slots[i].item.isStackable)
                {
                    if (slots[i].amount > 1)
                    {
                        slotUIs[i].itemAmountText.text = slots[i].amount.ToString();
                    }
                    else
                    {
                        slotUIs[i].itemAmountText.text = "";
                    }
                }
                else
                {
                    slotUIs[i].itemAmountText.text = "";
                }
            }
        }
    }
}
