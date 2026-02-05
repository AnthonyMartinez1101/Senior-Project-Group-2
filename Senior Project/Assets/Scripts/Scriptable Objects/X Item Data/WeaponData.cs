using UnityEngine;

public enum WeaponType
{
    Pistol,
    AR,
    Shotgun,
    Grenade
}

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Item Data/Weapon Data", order = -500)]    
public class WeaponData : ExtraItemData
{
    public WeaponType weaponType;
}
