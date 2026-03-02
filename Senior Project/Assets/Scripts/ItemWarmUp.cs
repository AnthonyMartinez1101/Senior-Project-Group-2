using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This is to "warm up" the item drop system, so that the first item drop doesn't cause a lag spike in the game
public class ItemWarmUp : MonoBehaviour
{
    public Item randItem;
    public List<GameObject> objectsToCreate = new List<GameObject>();
    private List<GameObject> objectsToDelete = new List<GameObject>();


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
        foreach (GameObject item in objectsToCreate)
        {
            if (!item) continue;

            GameObject obj = Instantiate(item, transform.position, Quaternion.identity);
            objectsToDelete.Add(obj);
        }

        yield return null;

        if (spawnedItem != null) Destroy(spawnedItem);

        foreach (GameObject item in objectsToDelete)
        {
            if (item != null) Destroy(item);
        }

        Destroy(gameObject);
    }
}
