using UnityEngine;

[CreateAssetMenu(fileName = "NewBucketData", menuName = "Item Data/Water Bucket Data", order = -500)]    
public class BucketData : ExtraItemData
{
    public Sprite fullSprite;
    public Sprite emptySprite;
    public int maxWater;
}
