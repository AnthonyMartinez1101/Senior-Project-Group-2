using UnityEngine;

public class BucketUpgrade : MonoBehaviour
{
    public Item[] bucketUpgrades;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerInventory = collision.GetComponent<Inventory>();
            if(playerInventory != null) playerInventory.UpgradeBucket(bucketUpgrades);
        }
    }
}
