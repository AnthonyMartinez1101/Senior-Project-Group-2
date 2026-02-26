using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    private List<Item> packagedItems = new List<Item>();

    public void CreatePackage(List<Item> items)
    {
        if (items == null || items.Count == 0) return;

        packagedItems.AddRange(items);
    }

    private void OnDestroy()
    {
        if (packagedItems.Count <= 0) return;

        // Drop all items in the package when it is destroyed
        foreach (Item item in packagedItems)
        {
            ItemDropFactory.Instance.SpawnItem(item, 0, transform.position, true);
        }
    }
}
