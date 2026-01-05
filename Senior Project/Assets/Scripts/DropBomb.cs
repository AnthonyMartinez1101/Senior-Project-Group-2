using UnityEngine;

public class DropBomb : MonoBehaviour
{
    public Item grenadeItem; // Reference to the bomb prefab

    public float dropTime = 60f;
    private float actualTimer = 0f;

    void Start()
    {
        actualTimer = dropTime;
    }


    // Update is called once per frame
    void Update()
    {
        actualTimer -= Time.deltaTime;

        if (actualTimer <= 0f)
        {
            ItemDropFactory.Instance.SpawnItem(grenadeItem, transform.position, expires: true);
            actualTimer = dropTime; // Reset to desired interval
        }
    }
}
