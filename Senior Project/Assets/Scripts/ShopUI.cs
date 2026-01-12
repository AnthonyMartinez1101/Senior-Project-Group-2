using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopUI : MonoBehaviour
{
    public GameObject[] buttons;

    public Image sellingIcon;
    public TMP_Text sellingAmount;

    private Inventory inventory;
    private PlayerWallet wallet;

    public Image redBox;




    public void Init(Inventory inv, PlayerWallet wall)
    {
        inventory = inv;
        wallet = wall;
        UpdateShopUI();
    }

    public void UpdateShopUI()
    {
        if (buttons == null || inventory == null || wallet == null || redBox == null) return;
        UpdateButtons();
        UpdateSellingBox();
    }

    private void UpdateSellingBox()
    {
        Item sellingItem = inventory.GetCurrentItem();

        if (sellingItem == null)
        {
            sellingAmount.enabled = false;
            sellingIcon.enabled = false;
            redBox.enabled = false;
            return;
        }

        sellingAmount.enabled = true;
        sellingAmount.text = "x" + sellingItem.sellPrice.ToString();

        sellingIcon.enabled = true;
        sellingIcon.sprite = sellingItem.icon;

        if(sellingItem.sellPrice <= 0) redBox.enabled = true;
        else redBox.enabled = false;
    }

    private void UpdateButtons()
    {
        int coins = wallet.GetCoinCount();

        for (int i = 0; i < buttons.Length; i++)
        {
            ShopButton shopButton = buttons[i].GetComponent<ShopButton>();
            if (shopButton != null)
            {
                Item item = shopButton.sellingItem;
                if (item != null)
                {
                    if (coins >= item.buyPrice)
                    {
                        shopButton.SetTextColor(Color.white);
                    }
                    else
                    {
                        shopButton.SetTextColor(Color.red);
                    }
                }
            }
        }
    }
}
