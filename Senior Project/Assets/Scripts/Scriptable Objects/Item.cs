using UnityEngine;

#if UNITY_EDITOR
using static UnityEngine.InputManagerEntry;
#endif

public enum ItemType
{
    Seed,
    Weapon,
    Produce,
    WaterCan
}

public enum WeaponType
{
    None,
    Pistol,
    Grenade
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Item", order = -500)]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    
    [Header("ITEM BEHAVIOR")]
    [Tooltip("If true, this item disappears when count reaches 0. If false, it stays in inventory (like a watering can).")]
    public bool isConsumable = true;

    [Header("--If Item Type Seed--")]
    [Tooltip("A plant scriptable object")]
    public PlantItem plant;

    [Header("--If Item Type Produce--")]
    [Tooltip("Health regen when eating produce")]
    public float healAmount;

    [Header("--If Item Type Weapon--")]
    [Tooltip("A weapon type")]
    //public WeaponItem weapon;
    public WeaponType weaponType;

    [Header("--Economy--")]
    [Tooltip("Amount of coins player receives when selling one unit of this item")]
    public int sellPrice = 0;

    //~~~~EDITOR EASE OF USE~~~~//
    private PlantItem savedPlant;
    private float savedHealAmount;
    private WeaponType savedWeaponType;

    //Runs only in the editor
    #if UNITY_EDITOR
    //OnValidate runs when the script has a change applied
    void OnValidate()
    {
        if (itemType != ItemType.Seed)
        {
            plant = null; // makes plant null when itemType is not seed (since any other type doesn't need a plant def)
        }
        if (plant != null)
        {
            savedPlant = plant; //Saves the plant reference so that changing itemType doesn't permanently delete the assigned plant
        }
        if(itemType == ItemType.Seed) 
        {
            plant = savedPlant; //Will re-add the original plant after switching itemType back to seed
        }



        if(itemType != ItemType.Produce)
        {
            healAmount = 0f; // makes healAmount 0 when itemType is not produce (since any other type doesn't need a heal amount)
        }
        if(healAmount != 0f)
        {
            savedHealAmount = healAmount; //Saves the healAmount so that changing itemType doesn't permanently delete the assigned healAmount
        }
        if(itemType == ItemType.Produce)
        {
            healAmount = savedHealAmount; //Will re-add the original healAmount after switching itemType back to produce
        }



        if (itemType != ItemType.Weapon)
        {
            weaponType = WeaponType.None; // makes weaponType None when itemType is not weapon (since any other type doesn't need a weapon type)
        }
        if(weaponType != WeaponType.None)
        {
            savedWeaponType = weaponType; //Saves the weaponType so that changing itemType doesn't permanently delete the assigned weaponType
        }
        if(itemType == ItemType.Weapon)
        {
            weaponType = savedWeaponType; //Will re-add the original weaponType after switching itemType back to weapon
        }
    }
    #endif
}
