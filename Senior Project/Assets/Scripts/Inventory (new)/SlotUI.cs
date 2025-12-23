using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    public Image background;
    public Image itemIcon;
    public TMP_Text itemAmountText;

    void Start()
    {
        background.color = Color.gray;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemAmountText.text = "";
    }
}
