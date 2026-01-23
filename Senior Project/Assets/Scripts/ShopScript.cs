using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ShopScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    bool isPlayerNearby = false;
    bool isShopInUse = false;

    InputAction openShopAction;
    InputAction closeShopAction;

    [SerializeField] private GameObject shopScreen; // Reference to the shop UI GameObject

    public Transform itemDropOff;

    public GameObject player;
    private PlayerWallet playerWallet;
    private Inventory inventory;

    private ShopUI shopUI;

    public bool interactable = true;

    public UnityEvent ShopUsedEvent;

    private ShopAudio shopAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false; 

        openShopAction = InputSystem.actions.FindAction("OpenShop");

        closeShopAction = InputSystem.actions.FindAction("Pause");

        if (player == null)
        {
            player = GameManager.Instance.player;
        }
        playerWallet = player.GetComponent<PlayerWallet>();
        inventory = player.GetComponent<Inventory>();

        if (playerWallet == null)
        {
            Debug.LogError("ShopScript: PlayerWallet reference not set");
        }

        shopUI = shopScreen.GetComponent<ShopUI>();

        InitShopUI();

        shopAudio = GetComponent<ShopAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerNearby && openShopAction.WasPressedThisFrame() && interactable) //E key to toggle shop
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
        if(isShopInUse && closeShopAction.WasPressedThisFrame()) //Escape key to close shop
        {
            CloseShop();
        }
    }

    void InitShopUI()
    {
        shopUI.Init(inventory, playerWallet);
    }

    void DisplayShop()
    {
        ShopUsedEvent.Invoke();
        shopScreen.SetActive(true);
        isShopInUse = true;
    }

    public void CloseShop()
    {
        shopScreen.SetActive(false);
        isShopInUse = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.enabled = true;
            isPlayerNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.enabled = false;
            isPlayerNearby = false;
        }
    }

    public bool IsShopInUse()
    {
        return isShopInUse; 
    }

    public void BuyItem(Item item)
    {
        //Check if item is null
        if(item == null)
        {
            Debug.LogWarning("BuyItem: item is null");
            return;
        }

        //Get item price
        int price = item.buyPrice;

        //Check if item is sellable
        if (price <= 0)
        {
            Debug.LogWarning("Item cannot be sold: " + item.itemName);
            return;
        }

        //Check if player has enough coins and spawn item if they do
        if (CheckPrice(price))
        {
            ItemDropFactory.Instance.SpawnItem(item, itemDropOff.position, expires: true);
            shopAudio.PlayBuyCoin();
        }


        ShopUsedEvent.Invoke();
    }


    //Handles pricing and ordering items
    private bool CheckPrice(int price)
    { 
        if(playerWallet.GetCoinCount() >= price)
        {
            playerWallet.RemoveCoins(price);
            return true;
        }
        else
        {
            Debug.Log("Not enough coins to complete purchase");
            return false;
        }
    }

    public void SellItem()
    {
        Item currentItem = inventory.GetCurrentItem();
        if(currentItem != null)
        {
            int sellPrice = currentItem.sellPrice;
            if(sellPrice > 0)
            {
                playerWallet.AddCoins(sellPrice);
                inventory.SubtractItem();
                shopAudio.PlayBuyCoin();
            }
            else
            {
                Debug.Log("Item cannot be sold: " + currentItem.itemName);
            }
        }
        else
        {
            Debug.Log("No item selected.");
        }

        ShopUsedEvent.Invoke();
    }

    public void SellAllItems()
    {
        Item currentItem = inventory.GetCurrentItem();
        if (currentItem != null)
        {
            int sellPrice = currentItem.sellPrice;
            if (sellPrice > 0)
            {
                int totalPrice = 0;
                while(inventory.GetCurrentItemCount() > 0)
                {
                    totalPrice += sellPrice;
                    inventory.SubtractItem();
                }
                playerWallet.AddCoins(totalPrice);
                shopAudio.PlaySellCoin();
            }
            else
            {
                Debug.Log("Item cannot be sold: " + currentItem.itemName);
            }
        }
        else
        {
            Debug.Log("No item selected.");
        }

        ShopUsedEvent.Invoke();
    }
}
