using UnityEngine;
using UnityEngine.InputSystem;

public class ShopScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite highlightedShopSprite;

    bool isPlayerNearby = false;
    bool isShopInUse = false;

    InputAction openShopAction;

    [SerializeField] private GameObject shopUI; // Reference to the shop UI GameObject


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = shopSprite;

        openShopAction = InputSystem.actions.FindAction("OpenShop");
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerNearby && openShopAction.WasPressedThisFrame()) //E key to toggle shop
        {
            if (!isShopInUse)
            {
                DisplayShop();
            }
            else
            {
                CloseShop();
            }
        }
    }

    void DisplayShop()
    {
        shopUI.SetActive(true);
        isShopInUse = true;
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        isShopInUse = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = highlightedShopSprite;
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = shopSprite;
            isPlayerNearby = false;
        }
    }

    public bool IsShopInUse()
    {
        return isShopInUse; 
    }
}
