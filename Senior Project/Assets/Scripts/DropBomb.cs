using UnityEngine;

public class DropBomb : MonoBehaviour
{
    public Item grenadeItem; // Reference to the bomb prefab

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
            ItemDropFactory.Instance.SpawnItem(grenadeItem, 0, transform.position, expires: true);
            actualTimer = dropTime; // Reset to desired interval
        }
    }
}
