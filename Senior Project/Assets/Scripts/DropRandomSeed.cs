using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SeedDrop
{
    public Item seedItem;

    [Range(1, 100)]
    public int dropChance = 10;
}

public class DropRandomSeed : MonoBehaviour
{
    [SerializeField] private List<SeedDrop> seedDrops = new List<SeedDrop>();

    public float dropTime = 60f;
    private float actualTimer = 0f;

    private WorldClock worldClock;

    void Start()
    {
        actualTimer = dropTime;

        if(worldClock == null) worldClock = GameManager.Instance.GetWorldClock();
    }


    // Update is called once per frame
    void Update()
    {
        actualTimer -= Time.deltaTime;

        if (actualTimer <= 0f && worldClock.IsDay())
        {
            DropRandSeed();
            actualTimer = dropTime; // Reset to desired interval
        }
    }

    void DropRandSeed()
    {
        int safety = 500;
        int safetyCount = 0;

        while(safetyCount++ < safety)
        {
            int randomIndex = Random.Range(0, seedDrops.Count);
            SeedDrop randomSeedDrop = seedDrops[randomIndex];

            int randNum = Random.Range(1, 101);
            if(randNum <= randomSeedDrop.dropChance)
            {
                // Drop the seed item
                ItemDropFactory.Instance.SpawnItem(randomSeedDrop.seedItem, 0, transform.position, true);
                return; // Exit the loop after a successful drop
            }
        }
        ItemDropFactory.Instance.SpawnItem(seedDrops[0].seedItem, 0, transform.position, true);
    }


}
