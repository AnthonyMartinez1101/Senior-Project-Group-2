using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class ShopScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite shopSprite;
    [SerializeField] private Sprite highlightedShopSprite;

    bool isPlayerNearby = false;
    bool isShopInUse = false;

    InputAction openShopAction;

    [SerializeField] private GameObject shopUI; // Reference to the shop UI GameObject

    public Transform itemDropOff;

    public GameObject player;
    private PlayerWallet playerWallet;
    private Inventory inventory;

    public bool interactable = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = shopSprite;

        openShopAction = InputSystem.actions.FindAction("OpenShop");

        if(player == null)
        {
            player = GameManager.Instance.player;
        }
        playerWallet = player.GetComponent<PlayerWallet>();
        inventory = player.GetComponent<Inventory>();

        if (playerWallet == null)
        {
            Debug.LogError("ShopScript: PlayerWallet reference not set");
        }
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

            // Try to set nearbyShop on player's inventory so right-click can sell
            InventorySystem inv = other.GetComponent<InventorySystem>();
            if (inv != null)
            {
                inv.nearbyShop = this;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.sprite = shopSprite;
            isPlayerNearby = false;

            // Clear nearbyShop reference on player
            InventorySystem inv = other.GetComponent<InventorySystem>();
            if (inv != null && inv.nearbyShop == this)
            {
                inv.nearbyShop = null;
            }
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
        if (CheckPrice(price)) ItemDropFactory.Instance.SpawnItem(item, itemDropOff.position, expires: true);
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
    }

    public void SellAllItems()
    {
        Item currentItem = inventory.GetCurrentItem();
        if (currentItem != null)
        {
            int sellPrice = currentItem.sellPrice;
            if (sellPrice > 0)
            {
                while(inventory.GetCurrentItemCount() > 0)
                {
                    playerWallet.AddCoins(sellPrice);
                    inventory.SubtractItem();
                }
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
    }

    // Sell item from player's inventory when shop is open
    public bool SellItemFromInventory(InventorySystem inventory, int slotIndex)
    {
        if (!IsShopInUse())
        {
            Debug.Log("Shop is not open. Cannot sell items.");
            return false;
        }

        if (inventory == null)
        {
            Debug.LogError("SellItemFromInventory: inventory reference is null");
            return false;
        }

        Item itemToSell = inventory.GetItemAtSlot(slotIndex);
        int itemCount = inventory.GetItemCountAtSlot(slotIndex);

        if (itemToSell == null || itemCount <= 0)
        {
            Debug.Log("No item in selected slot to sell.");
            return false;
        }

        if (itemToSell.sellPrice <= 0)
        {
            Debug.Log("Item cannot be sold or has no sell price: " + itemToSell.itemName);
            return false;
        }

        // Add coins to player's wallet and remove one from inventory
        playerWallet.AddCoins(itemToSell.sellPrice);
        inventory.SubtractItemAtSlot(slotIndex, 1);

        Debug.Log("Sold 1 x " + itemToSell.itemName + " for " + itemToSell.sellPrice + " coins.");
        return true;
    }
}
