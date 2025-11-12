using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 1;
    public float knockbackForce = 5f;
    public enum WeaponType { Melee, Bullet }
    public WeaponType weaponType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("NoBulletCollision") || collision.CompareTag("Interact")) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage);

            if(collision.GetComponent<Knockback>()) collision.GetComponent<Knockback>().ApplyKnockback(transform, knockbackForce);
        }

            PlantScript plantScript = collision.GetComponent<PlantScript>();
        if (plantScript != null)
        {
            plantScript.TakeDamage(damage);
        }

        if (weaponType == WeaponType.Bullet)
        {
            Debug.Log("Bullet destroyed by: " + collision.name);
            Destroy(gameObject);
        }
    }
}
