using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractScript : MonoBehaviour, ICheat
{
    private Inventory inventorySystem;
    private Attack attack;

    InputAction interactAction;

    private Item currentItem;

    private SoilScript currentSoil;

    private PlayerHealth playerHealth;

    private MovementScript movementScript;

    private bool nearRefill;

    [SerializeField] private ShopScript shop;

    private bool isThrowing = false;
    private float maxChargeTime = 2.0f;
    private float chargeTime = 0.0f;
    public Slider grenadeSlider;

    public bool canPlant = true;
    public bool canWater = true;
    public bool canEat = true;

    private PlayerAudio playerAudio;

    private bool cheatMode = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventorySystem = GetComponent<Inventory>();
        attack = GetComponent<Attack>();

        interactAction = InputSystem.actions.FindAction("Interact");
        
        playerHealth = GetComponent<PlayerHealth>();
        movementScript = GetComponent<MovementScript>();

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

        playerAudio = GetComponent<PlayerAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactAction.WasPressedThisFrame() && shop != null && !shop.IsShopInUse())
        {
            //Return if pointer is over UI when clicked
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

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

    public void SetCheats()
    {
        cheatMode = true;
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
        if(!canEat) return;
        if (inventorySystem.GetCurrentItemCount() > 0)
        {
            var produceData = currentItem.extraItemData as ProduceData;
            if (produceData != null)
            {
                ProduceBuff(produceData.buffType);
                playerHealth.Heal(produceData.healAmount);
                if(!cheatMode) inventorySystem.SubtractItem();
                playerAudio.PlayMunch();
            }
        }
    }
    private void ProduceBuff(BuffType currentBuff)
    {
        // Implement buff logic based on the type of buff
            switch (currentBuff)
            {
                case BuffType.Speed:
                    movementScript.SpeedBuff();
                break;
    
                case BuffType.Damage:
                    attack.AttackBuff();
                    break;
    
                case BuffType.MaxHealth:
                    playerHealth.HealthBuff();
                break;
    
                default:
                    Debug.Log("No valid buff type.");
                    break;
        }
    }
    //Plant seed only if soil is highlighted and you have seeds in inventory
    private void PlantSeed()
    {
        if(!canPlant) return;
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
        if(!canWater) return;
        if (nearRefill)
        {
            inventorySystem.RefillWater();
            playerAudio.PlayWaterRefill();
            return;
        }
        if (currentSoil != null && currentSoil.IsHighlighted() && inventorySystem.GetCurrentItemCount() > 0)
        {
            currentSoil.Water();
            inventorySystem.UseWater();
            playerAudio.PlayWatering();
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
                ShootPistol();
                break;

            case WeaponType.AR:
                ShootAR();
                break;

            case WeaponType.Shotgun:
                ShootShotGun();
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

    private void ShootPistol()
    {
        if(cheatMode)
        {
            attack.OnShoot();
            return;
        }

        if (attack.CanShoot())
        {
            if (inventorySystem.CheckAndUseBullets(1))
            {
                attack.OnShoot();
            }
        }
    }

    private void ShootAR()
    {
        if(cheatMode)
        {
            attack.OnShootBurst();
            return;
        }

        if (attack.CanShoot())
        {
            if (inventorySystem.CheckAndUseBullets(3))
            {
                attack.OnShootBurst();
            }
        }
    }

    private void ShootShotGun()
    {
        if(cheatMode)
        {
            attack.OnShootShotgun();
            return;
        }

        if (attack.CanShoot())
        {
            if (inventorySystem.CheckAndUseBullets(3))
            {
                attack.OnShootShotgun();
            }
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
