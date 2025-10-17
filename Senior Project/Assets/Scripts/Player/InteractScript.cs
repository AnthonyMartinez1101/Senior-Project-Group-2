using UnityEngine;
using UnityEngine.InputSystem;

public class InteractScript : MonoBehaviour
{
    private InventorySystem inventorySystem;
    private Attack attack;

    InputAction interactAction;

    private Item currentItem;

    private SoilScript currentSoil;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventorySystem = GetComponent<InventorySystem>();
        attack = GetComponent<Attack>();

        interactAction = InputSystem.actions.FindAction("Interact");
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
                Debug.Log("Using produce");
                break;

            case ItemType.WaterCan:
                WaterSoil();
                break;

            default:
                Debug.Log("No valid item to interact with.");
                break;
        }
    }

    private void PlantSeed()
    {
        if (currentSoil.IsHighlighted())
        {
            if (currentSoil.Plant(currentItem))
            {
                inventorySystem.SubtractItem();
            }
        }
    }

    private void WaterSoil()
    {
        if(currentSoil.IsHighlighted()) currentSoil.Water();
    }

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
