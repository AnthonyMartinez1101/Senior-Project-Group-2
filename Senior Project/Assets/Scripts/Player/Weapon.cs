using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 1;
    public float knockbackForce = 5f;
    public enum WeaponType { Sickle, Bullet, Grenade }
    public WeaponType weaponType;

    public enum BulletType { NA, Pistol, AR, Shotgun };
    public BulletType bulletType;

    private float bulletDamageChange = 30f;

    private float shotgunBulletWait = 0.012f;

    public float explosionTimer = 2.5f;
    public Collider2D explosionRadius;

    private Animator animator;

    public int bulletHitCount = 5;
    private int bulletTotalHit = 0;

    private float timeAlive = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (weaponType == WeaponType.Grenade)
        {
            StartCoroutine(Explode(explosionTimer));
        }

        if(bulletType == BulletType.Shotgun)
        {
            damage *= 3;
        }

    }

    void Update()
    {
        if (bulletType == BulletType.NA) return;

        timeAlive += Time.deltaTime;

        //Damage over time behavior depending on gun type
        switch(bulletType)
        {
            //Persistent damage for pistols
            case BulletType.Pistol:
                break;

            //Increasing damage for ARs (max of 6 damage)
            case BulletType.AR:
                damage += bulletDamageChange * Time.deltaTime;
                damage = Mathf.Min(damage, 6);
                break;

            //Decreasing damage for shotguns (min of 1 damage) 
            case BulletType.Shotgun:
                shotgunBulletWait -= Time.deltaTime;
                if(shotgunBulletWait <= 0)
                {
                    damage = 1f;
                }
                break;
        }
    }

    private bool Avoid(string tag)
    {
        //Strings of colliders to avoid if it sneaks past the layer mask
        return tag == "Bullet" ||
               tag == "Player" ||
               tag == "NoBulletCollision" ||
               tag == "Interact" ||
               tag == "Shadow";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Avoid(collision.tag)) return;

        //Find if collided object is damageable
        var damageable = collision.GetComponent<IDamageable>() ?? collision.GetComponentInParent<IDamageable>() ?? collision.GetComponentInChildren<IDamageable>();

        if (damageable != null)
        {
            //Gets damagetype which is currently only used by plants with DamageType.Sickle (made to be modular though)
            DamageType damageType = weaponType switch
            {
                WeaponType.Sickle => DamageType.Sickle,
                WeaponType.Bullet => DamageType.Bullet,
                WeaponType.Grenade => DamageType.Explosion,
                _ => DamageType.Enemy
            };

            //Deal damage, increment bullet hit count
            damageable.TakeDamage(damage, damageType);
            bulletTotalHit++;

            //Apply knockback if object has knockback component
            Knockback knockback = collision.GetComponent<Knockback>();
            if (knockback != null) knockback.ApplyKnockback(transform, knockbackForce);
        }

        //Bullet destroy condition
        if (weaponType == WeaponType.Bullet && (bulletTotalHit == 0 || bulletTotalHit == bulletHitCount))
        {
            //Debug.Log("Bullet destroyed by: " + collision.name + "\nCurrent damage: " + damage + "\nTime alive: " + timeAlive);
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
