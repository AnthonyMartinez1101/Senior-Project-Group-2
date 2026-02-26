using System.Collections;
using UnityEngine;

//This is to "warm up" the item drop system, so that the first item drop doesn't cause a lag spike in the game
public class ItemWarmUp : MonoBehaviour
{
    public Item randItem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitToDestroy());
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.CameraShake(0f, 0f);
        GameObject spawnedItem = ItemDropFactory.Instance.SpawnItem(randItem, 0, transform.position, true);
        yield return null;
        if (spawnedItem != null) Destroy(spawnedItem);
        Destroy(gameObject);
    }
}
