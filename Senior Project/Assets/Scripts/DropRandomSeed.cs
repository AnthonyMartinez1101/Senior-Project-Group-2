using UnityEngine;

public class DropRandomSeed : MonoBehaviour
{
    public Item[] normalDrop;
    public Item[] rareDrop;

    [Range(0, 100)]
    public int rareDropChance = 10;

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
            int randNum = Random.Range(1, 101);
            Item[] chosenItem = (randNum <= rareDropChance) ? rareDrop : normalDrop;
            if(chosenItem != null && chosenItem.Length > 0)
            {
                Item randomItem = chosenItem[Random.Range(0, chosenItem.Length)];
                if(randomItem != null)
                {
                    ItemDropFactory.Instance.SpawnItem(randomItem, 0, transform.position, expires: true);
                }
            }
            actualTimer = dropTime; // Reset to desired interval
        }
    }


}
