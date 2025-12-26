using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

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
        for (int i = 0; i < slotUIs.Count; i++)
        {
            // Highlight slot if currently selected
            if (i == currentlySelected) slotUIs[i].background.color = Color.yellow;
            else slotUIs[i].background.color = Color.gray;

            //If slot has item, set item icon
            if (slots[i].item != null)
            {
                slotUIs[i].itemIcon.enabled = true;
                //If item is not water can, set normal item icon
                if (slots[i].item.itemType != ItemType.WaterCan) slotUIs[i].itemIcon.sprite = slots[i].item.icon;

                //If item is water can...
                else
                {
                    //..get bucket info
                    var bucketData = slots[i].item.extraItemData as BucketData;
                    if (bucketData != null)
                    {
                        //...set icon based on water amount
                        if (slots[i].waterAmount > 0) slotUIs[i].itemIcon.sprite = bucketData.fullSprite;
                        else slotUIs[i].itemIcon.sprite = bucketData.emptySprite;
                    }
                    //...if no bucket data, just set normal item icon
                    else slotUIs[i].itemIcon.sprite = slots[i].item.icon;
                }
            }
            //If slot is empty, remove item icon
            else
            {
                slotUIs[i].itemIcon.sprite = null;
                slotUIs[i].itemIcon.enabled = false;
            }


            
            if (slots[i].amount <= 0 || !slots[i].item.isStackable) slotUIs[i].itemAmountText.text = "";
            else if (slots[i].item.itemType == ItemType.WaterCan) slotUIs[i].itemAmountText.text = "x" + slots[i].waterAmount.ToString();
            else slotUIs[i].itemAmountText.text = "x" + slots[i].amount.ToString();
        }
    }
}
