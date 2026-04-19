using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections.Generic;

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

    public GameObject packagePrefab;
    private GameObject currentPackage;
    private List<Item> purchasedItems = new List<Item>();

    private ShopUI shopUI;

    public bool interactable = true;

    public UnityEvent ShopUsedEvent;
    public UnityEvent OnAnyPurchase;
    public UnityEvent OnShopClosed;


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

        if(packagePrefab == null)
        {
            Debug.LogError("ShopScript: Package prefab reference not set");
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
        purchasedItems.Clear();
    }

    public void CloseShop()
    {
        inventory.ReturnShopSlotToInv();
        shopScreen.SetActive(false);
        isShopInUse = false;

        CreatePackage();

        OnShopClosed?.Invoke(); // Invoke the shop closed event for any listeners
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

    private void CreatePackage()
    {
        if (purchasedItems.Count > 0)
        {
            if(currentPackage == null) currentPackage = Instantiate(packagePrefab, itemDropOff.position, Quaternion.identity);

            var package = currentPackage.GetComponent<Package>();
            if(package) package.CreatePackage(purchasedItems);
        }
        purchasedItems.Clear();
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
            purchasedItems.Add(item);
            shopAudio.PlayBuyCoin();

            OnAnyPurchase?.Invoke(); // Invoke the purchase event for any listeners
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
        Slot slot = inventory.GetSellSlot();
        Item currentItem = slot.item;

        if (currentItem != null)
        {
            int sellPrice = currentItem.sellPrice;
            if(sellPrice > 0)
            {
                playerWallet.AddCoins(sellPrice);
                inventory.SubtractSellSlot();
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
        Slot slot = inventory.GetSellSlot();
        Item currentItem = slot.item;

        if (currentItem != null)
        {
            int sellPrice = currentItem.sellPrice;
            if (sellPrice > 0)
            {
                int totalPrice = 0;
                int breakpoint = 0;
                while(slot.amount > 0)
                {
                    totalPrice += sellPrice;
                    inventory.SubtractSellSlot();

                    breakpoint++;
                    if(breakpoint > 1000)
                    {
                        Debug.LogError("SellAllItems: Infinite loop detected");
                        break;
                    }
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
