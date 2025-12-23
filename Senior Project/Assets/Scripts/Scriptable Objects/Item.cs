using UnityEngine;

public enum ItemType
{
    Seed,
    Weapon,
    Produce,
    WaterCan
}

public abstract class ExtraItemData : ScriptableObject { }

[CreateAssetMenu(fileName = "NewItem", menuName = "Item", order = -500)]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    
    [Header("ITEM BEHAVIOR")]
    [Tooltip("If true, this item disappears when count reaches 0. If false, it stays in inventory (like a watering can).")]
    public bool isConsumable = true;
    public bool isStackable = true;

    [Header("--Economy--")]
    [Tooltip("Amount of coins player receives when selling one unit of this item")]
    public int sellPrice = 0;
    public int buyPrice = 0;

    [Header("--Extra Data--")]
    [Tooltip("If extra functionality to a specific item type is needed")]
    public ExtraItemData extraItemData;

    //To check for extraItemData in other scripts, do:
    //var <varName> = item.extraItemData as <data type you are finding>;
}
