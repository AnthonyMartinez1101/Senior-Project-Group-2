using UnityEngine;

//Makes asset in the menu. -500 ensures it appears at the top
[CreateAssetMenu(fileName = "NewPlant", menuName = "Plant", order = -500)]
public class PlantItem : ScriptableObject
{
    public string plantName;
    public float growthTime;

    [Header("Seeds ~ Stage 1 - Stage 2 - Stage 3")]
    public Sprite[] growStages = new Sprite[4];

    [Header("Produce Plant Item")]
    public Item produce;

    [Header("Seed Item")]
    public Item seed;

    [Header("Range Of Random Produce Drops")]
    public int minDrop = 1;
    public int maxDrop = 1;

    [Header("% Chance To Drop Seed On Harvest")]
    [Range(0, 100)] 
    public int seedChance = 0;

    [Header("Can Plant Die to Dehydration")]
    public bool canDry = true;
}
