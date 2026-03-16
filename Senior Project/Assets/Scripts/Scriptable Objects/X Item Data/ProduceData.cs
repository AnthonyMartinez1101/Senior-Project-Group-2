using UnityEngine;

public enum BuffType
{
    Speed,
    Damage,
    MaxHealth
}

[CreateAssetMenu(fileName = "NewProduceData", menuName = "Item Data/Produce Data", order = -500)]    
public class ProduceData : ExtraItemData
{
    public float healAmount;
    public BuffType buffType;
}
