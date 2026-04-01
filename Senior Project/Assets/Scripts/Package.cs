using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour, IDamageable
{
    [SerializeField] float health, maxHealth = 5f;
    private FloatingHealth healthBar;

    private List<Item> packagedItems = new List<Item>();

    public void CreatePackage(List<Item> items)
    {
        if (items == null || items.Count == 0) return;

        health = maxHealth;
        healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.UpdateHealth(health, maxHealth);

        packagedItems.AddRange(items);
    }

    public void TakeDamage(float damageAmount, DamageType weaponType)
    {
        if (damageAmount <= 0) return;

        health -= damageAmount;
        if (healthBar) healthBar.UpdateHealth(health, maxHealth);
        if (health <= 0)
        {
            OpenPackage();
        }
    }

    void OpenPackage()
    {
        if (packagedItems.Count <= 0) return;

        // Drop all items in the package when it is destroyed
        foreach (Item item in packagedItems)
        {
            ItemDropFactory.Instance.SpawnItem(item, 0, transform.position, true);
        }
        Destroy(gameObject);
    }
}
