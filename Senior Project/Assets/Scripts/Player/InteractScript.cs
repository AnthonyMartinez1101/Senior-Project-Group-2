using UnityEngine;
using UnityEngine.InputSystem;

public class InteractScript : MonoBehaviour
{
    private InventorySystem inventorySystem;
    private Attack attack;

    InputAction interactAction;

    private Item currentItem;

    private SoilScript currentSoil;

    private PlayerHealth playerHealth;

    private bool nearRefill;

    [SerializeField] private ShopScript shop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventorySystem = GetComponent<InventorySystem>();
        attack = GetComponent<Attack>();

        interactAction = InputSystem.actions.FindAction("Interact");
        
        playerHealth = GetComponent<PlayerHealth>();

        nearRefill = false;

        if (shop == null)
        {
            Debug.Log("InteractScript: Shop not assigned in inspector, searching for ShopScript.");
            shop = FindObjectOfType<ShopScript>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(interactAction.WasPressedThisFrame() && shop != null && !shop.IsShopInUse())
        {
            Interact();
        }
    }

    public void SetSoil(SoilScript newSoil)
    {
        currentSoil = newSoil;
    }

    // Handle interaction based on current item type when interact button is pressed
    private void Interact()
    {
        currentItem = inventorySystem.GetCurrentItem();

        if (currentItem == null) return;

        switch(currentItem.itemType)
        {
            case ItemType.Seed:
                PlantSeed();
                break;

            case ItemType.Weapon:
                DoAttack();
                break;

            case ItemType.Produce:
                EatProduce();
                break;

            case ItemType.WaterCan:
                TryWater();
                break;

            default:
                Debug.Log("No valid item to interact with.");
                break;
        }
    }

    //Eat produce only if you have produce in hand and are not at max health
    private void EatProduce()
    {
        if(inventorySystem.GetCurrentItemCount() > 0 && !playerHealth.IsMaxHealth())
        {
            playerHealth.Heal(currentItem.healAmount);
            inventorySystem.SubtractItem();
        }
    }

    //Plant seed only if soil is highlighted and you have seeds in inventory
    private void PlantSeed()
    {
        if (currentSoil != null && currentSoil.IsHighlighted() && inventorySystem.GetCurrentItemCount() > 0)
        {
            if (currentSoil.Plant(currentItem))
            {
                inventorySystem.SubtractItem();
            }
        }
    }

    // Water soil only if soil is highlighted
    private void TryWater()
    {
        if(nearRefill)
        {
            inventorySystem.RefillWater();
            return;
        }
        if (currentSoil != null && currentSoil.IsHighlighted() && inventorySystem.GetCurrentItemCount() > 0)
        {
            currentSoil.Water();
            inventorySystem.SubtractItem();
        }
    }

    // Perform attack based on weapon type
    private void DoAttack()
    {
        switch(currentItem.weaponType)
        {
            case WeaponType.Pistol:
                attack.OnShoot();
                break;

            default:
                Debug.Log("No valid weapon type to attack with.");
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Refill"))
        {
            nearRefill = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Refill"))
        {
            nearRefill = false;
        }
    }
}
