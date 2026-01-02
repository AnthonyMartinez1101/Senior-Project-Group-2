using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractScript : MonoBehaviour
{
    private Inventory inventorySystem;
    private Attack attack;

    InputAction interactAction;

    private Item currentItem;

    private SoilScript currentSoil;

    private PlayerHealth playerHealth;

    private bool nearRefill;

    [SerializeField] private ShopScript shop;

    private bool isThrowing = false;
    private float maxChargeTime = 2.0f;
    private float chargeTime = 0.0f;
    public Slider grenadeSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventorySystem = GetComponent<Inventory>();
        attack = GetComponent<Attack>();

        interactAction = InputSystem.actions.FindAction("Interact");
        
        playerHealth = GetComponent<PlayerHealth>();

        nearRefill = false; 

        if (shop == null)
        {
            Debug.Log("InteractScript: Shop not assigned in inspector");
        }

        if (grenadeSlider != null)
        {
            grenadeSlider.maxValue = maxChargeTime;
            grenadeSlider.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (interactAction.WasPressedThisFrame() && shop != null && !shop.IsShopInUse())
        {
            currentItem = inventorySystem.GetCurrentItem();
            if (currentItem != null && currentItem.itemType == ItemType.Weapon)
            {
                var weaponData = currentItem.extraItemData as WeaponData;
                if (weaponData != null && weaponData.weaponType == WeaponType.Grenade)
                {
                    grenadeSlider.gameObject.SetActive(true);
                    grenadeSlider.value = 0.0f;
                    isThrowing = true;
                    chargeTime = Time.time;
                    return;
                }
            }
            Interact();
        }
        if (isThrowing)
        {
            float currentCharge = Mathf.Min(Time.time - chargeTime, maxChargeTime);
            grenadeSlider.value = currentCharge;
            if (interactAction.WasReleasedThisFrame())
            {
                isThrowing = false;
                grenadeSlider.gameObject.SetActive(false);
                Interact();
            }
        }
    }

    public void SetSoil(SoilScript newSoil)
    {
        currentSoil = newSoil;
    }

    // Handle interaction based on current item type when interact button is pressed
    private void Interact()
    {
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
            var produceData = currentItem.extraItemData as ProduceData;
            if (produceData != null)
            {
                playerHealth.Heal(produceData.healAmount);
                inventorySystem.SubtractItem();
            }
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
            inventorySystem.UseWater();
        }
    }

    // Perform attack based on weapon type
    private void DoAttack()
    {
        var weaponData = currentItem.extraItemData as WeaponData;
        if (weaponData == null) return;

        switch (weaponData.weaponType)
        {
            case WeaponType.Pistol:
                Shoot();
                break;

            case WeaponType.Grenade:
                if (inventorySystem.GetCurrentItemCount() > 0)
                {
                    inventorySystem.SubtractItem();
                    attack.OnThrowGrenade(Mathf.Min(Time.time - chargeTime, maxChargeTime));
                    chargeTime = 0.0f;
                }
                break;

            default:
                Debug.Log("No valid weapon type to attack with.");
                break;
        }
    }

    private void Shoot()
    {
        if(inventorySystem.CheckAndUseBullets()) attack.OnShoot();
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
