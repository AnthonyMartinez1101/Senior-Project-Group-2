using System.Collections;
using UnityEngine;

public class ItemDropFactory : MonoBehaviour
{
    public static ItemDropFactory Instance;
    public GameObject itemDropPrefab;

    public Transform GroundItemCollection;

    [SerializeField] private Item[] randomDropItems;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SpawnItem(Item item, int runtimeVal, Vector3 pos, bool expires)
    {
        GameObject itemDrop = Instantiate(itemDropPrefab, pos, Quaternion.identity, GroundItemCollection);
        itemDrop.GetComponent<ItemDropScript>().CreateItemDrop(item, runtimeVal);
        
        if (!expires) itemDrop.GetComponent<ItemDropScript>().canDespawn = false;
        itemDrop.GetComponent<ItemDropScript>().canDespawn = (item.itemType != ItemType.WaterCan);

        Rigidbody2D rb = itemDrop.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(1f, 4f), ForceMode2D.Impulse);
    }

    public void PlayerDropItem(Item item, int runtimeVal, Vector3 pos)
    {
        GameObject itemDrop = Instantiate(itemDropPrefab, pos, Quaternion.identity, GroundItemCollection);
        itemDrop.GetComponent<ItemDropScript>().canDespawn = (item.itemType != ItemType.WaterCan);
        itemDrop.GetComponent<CircleCollider2D>().enabled = false; // Disable collider to prevent immediate re-pickup
        itemDrop.GetComponent<ItemDropScript>().CreateItemDrop(item, runtimeVal);
        Rigidbody2D rb = itemDrop.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(2f, 4f), ForceMode2D.Impulse);
        StartCoroutine(WaitToEnable(itemDrop));
    }

    public void SpawnRandomItem(Item item, Vector3 pos)
    {
        int randIndex = Random.Range(0, randomDropItems.Length);
        GameObject itemDrop = Instantiate(itemDropPrefab, pos, Quaternion.identity, GroundItemCollection);
        itemDrop.GetComponent<ItemDropScript>().CreateItemDrop(randomDropItems[randIndex], 0);

        Rigidbody2D rb = itemDrop.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f)).normalized * Random.Range(2f, 5f), ForceMode2D.Impulse);
    }

    private IEnumerator WaitToEnable(GameObject itemDrop)
    {
        yield return new WaitForSeconds(1f);
        itemDrop.GetComponent<CircleCollider2D>().enabled = true;
    }
}
