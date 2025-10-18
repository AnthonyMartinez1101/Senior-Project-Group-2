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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventorySystem = GetComponent<InventorySystem>();
        attack = GetComponent<Attack>();

        interactAction = InputSystem.actions.FindAction("Interact");
        
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if(interactAction.WasPressedThisFrame())
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
                WaterSoil();
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
            inventorySystem.UpdateDisplayText();
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
                inventorySystem.UpdateDisplayText();
            }
        }
    }

    // Water soil only if soil is highlighted
    private void WaterSoil()
    {
        if(currentSoil != null && currentSoil.IsHighlighted()) currentSoil.Water();
    }

    // Perform attack based on weapon type
    private void DoAttack()
    {
        switch(currentItem.weaponType)
        {
            case WeaponType.Sicky:
                attack.OnMelee();
                break;

            case WeaponType.Gun:
                attack.OnShoot();
                break;

            default:
                Debug.Log("No valid weapon type to attack with.");
                break;
        }
    }
}
