using UnityEngine;
using UnityEngine.UI;

public class changeIcon : MonoBehaviour
{
    [SerializeField] private Sprite emptyCart;
    [SerializeField] private Sprite fullCart;

    private Image img;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        img = GetComponent<Image>();
        SetEmptyCart();
    }
    public void SetEmptyCart()
    {
        if (img != null && emptyCart != null)
        {
            img.sprite = emptyCart;
        }
    }

    public void SetFullCart()
    {
        if (img != null && fullCart != null)
        {
            img.sprite = fullCart;
        }
    }
}