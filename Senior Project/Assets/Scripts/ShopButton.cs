using UnityEngine;
using TMPro;

public class ShopButton : MonoBehaviour
{
    public Item sellingItem;
    public TMP_Text costText;

    public void SetTextColor(Color color)
    {
        if (costText != null)
        {
            costText.color = color;
        }
    }
}
