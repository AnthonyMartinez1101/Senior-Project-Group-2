using System.Collections;
using UnityEngine;

public class ChickenExplosion : MonoBehaviour
{
    public GameObject explosionParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        GameManager.Instance.CameraShake(20f, 0.5f);
        GameObject particles = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(particles, 10f);
        yield return new WaitForSeconds(0.2f); // Wait for 0.2 seconds before exploding
        Destroy(gameObject);
        GameManager.Instance.CameraShake(20f, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            //Instakill anything in radius wink-wink
            damageable.TakeDamage(9999f, DamageType.Explosion);
        }
    }
}
