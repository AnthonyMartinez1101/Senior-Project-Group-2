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

    //List of items the shop sells
    public List<Item> sellableItems = new List<Item>();

    public Transform itemDropOff;

    public PlayerWallet playerWallet;

    private bool foundItem = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = shopSprite;

        openShopAction = InputSystem.actions.FindAction("OpenShop");

        if(playerWallet == null)
        {
            Debug.LogError("ShopScript: PlayerWallet reference not set");
        }
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


    //Order functions for buttons in the shop UI
    public void OrderCorn()
    {
        if(CheckPrice(1)) OrderItem("Corn Seed");
    }
    public void OrderPistol()
    {
        if(CheckPrice(100)) OrderItem("Pistol");
    }
    public void OrderPistolSeed()
    {
        if (CheckPrice(10)) OrderItem("Pistol Seed");
    }
    public void OrderGrenade()
    {
        if (CheckPrice(20)) OrderItem("Grenade");
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
    private void OrderItem(string orderName)
    {
        foundItem = false;
        for (int i = 0; i < sellableItems.Count; i++)
        {
            if (sellableItems[i].itemName == orderName)
            {
                ItemDropFactory.Instance.SpawnItem(sellableItems[i], itemDropOff.position);
                foundItem = true;
            }
        }
        if(!foundItem) Debug.Log("Cannot find item: " + orderName);
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
