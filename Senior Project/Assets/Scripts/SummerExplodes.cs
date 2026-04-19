using UnityEngine;
using System.Collections;

public class SummerExplodes : MonoBehaviour
{
    public Collider2D explosionRadius;
    [SerializeField] private int damage = 3;
    [SerializeField] private float lifetime = 0.25f;
    [SerializeField] private float delay = 1f;

    private CircleCollider2D col;
    public GameObject explosionEffect;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        StartCoroutine(ArmThenDestroy());
    }

    private IEnumerator ArmThenDestroy()
    {
        if(col != null) col.enabled = false;
        yield return new WaitForSeconds(delay);

        if(col != null) col.enabled = true;
        yield return new WaitForSeconds(lifetime);

        if (explosionEffect) Instantiate(explosionEffect, transform.position, transform.rotation);
        GameManager.Instance.CameraShake(15f, 0.5f);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damageable = collision.GetComponent<IDamageable>();
        if(damageable != null)
        {
            damageable.TakeDamage(damage, DamageType.Explosion);
        }

        var knockback = collision.GetComponent<Knockback>();
        if(knockback != null)
        {
            knockback.ApplyKnockback(transform, 20f);
        }
    }
}
