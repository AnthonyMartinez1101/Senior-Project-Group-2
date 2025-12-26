using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;  


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


    //*Function made with the help of ChatGPT*
    public void Bob()
    {
        if (!itemIcon || !itemIcon.enabled || itemIcon.sprite == null) return;

        var rt = itemIcon.rectTransform;
        rt.DOKill(true); // kill running tweens on this rect

        rt.localScale = Vector3.one;

        rt.DOScale(1.2f, 0.08f)
          .SetEase(Ease.OutBack)
          .SetUpdate(true)
          .OnComplete(() =>
              rt.DOScale(1f, 0.10f).SetEase(Ease.InOutQuad).SetUpdate(true)
          );
    }
}
