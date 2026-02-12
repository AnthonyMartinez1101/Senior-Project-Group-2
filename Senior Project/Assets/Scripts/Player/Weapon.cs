using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 1;
    public float knockbackForce = 5f;
    public enum WeaponType { Melee, Bullet, Grenade }
    public WeaponType weaponType;

    public float explosionTimer = 2.5f;
    public Collider2D explosionRadius;

    private Animator animator;

    public int bulletHitCount = 5;
    private int bulletTotalHit = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (weaponType == WeaponType.Grenade)
        {
            StartCoroutine(Explode(explosionTimer));
        }
        
    }

    private bool Avoid(string tag)
    {
        return tag == "Player" || 
               tag == "NoBulletCollision" || 
               tag == "Interact" ||
               tag == "Shadow" ||
               tag == "Bullet";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Avoid(collision.tag)) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage);
            bulletTotalHit++;

            if(collision.GetComponent<Knockback>()) collision.GetComponent<Knockback>().ApplyKnockback(transform, knockbackForce);
        }

        BossScript boss = collision.GetComponent<BossScript>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            bulletTotalHit++;
            if (collision.GetComponent<Knockback>()) collision.GetComponent<Knockback>().ApplyKnockback(transform, knockbackForce);
        }

        PlantScript plantScript = collision.GetComponent<PlantScript>();
        if (plantScript != null)
        {
            plantScript.TakeDamage(damage);
            bulletTotalHit++;
        }

        if (weaponType == WeaponType.Bullet && (bulletTotalHit == 0 || bulletTotalHit == bulletHitCount))
        {
            Debug.Log("Bullet destroyed by: " + collision.name);
            Destroy(gameObject);
        }
    }

    private IEnumerator Explode(float delay)
    {
        //This was throwing a warning and it doesn't seem to do anything so I'm commenting it out
        //animator.Play("Boom");
        yield return new WaitForSeconds(delay);
        explosionRadius.enabled = true;
        Destroy(gameObject, 0.1f);
    }
}
