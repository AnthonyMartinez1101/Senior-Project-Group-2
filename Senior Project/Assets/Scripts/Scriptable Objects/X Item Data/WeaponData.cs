using UnityEngine;

public enum WeaponType
{
    Pistol,
    Grenade
}

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Item Data/Weapon Data", order = -500)]    
public class WeaponData : ExtraItemData
{
    public WeaponType weaponType;
}
