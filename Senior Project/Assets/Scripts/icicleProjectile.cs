using UnityEngine;
using System.Collections;

public class icicleProjectile : MonoBehaviour
{
    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float slowDuration = 1f;

    [SerializeField] private float lifetime = 3f;

    private bool hasHit = false;

    private Collider2D col;
    private SpriteRenderer sr; // made these so that we can disable them on hit instead of destroying the object, which allows the slow effect to persist visually
    private Rigidbody2D rb;

    private void Awake() 
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        StartCoroutine(DestroyIfNoHit()); // Start the coroutine to destroy the projectile if it doesn't hit anything 
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (hasHit) return;
        if (!collider.CompareTag("Player")) return;
        hasHit = true;
        MovementScript movement = collider.GetComponent<MovementScript>();
        if (movement == null || movement.IsSlowed())
        {
            Destroy(gameObject);
            return;
        }
        if (col != null) col.enabled = false;
        if (sr != null) sr.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
        StartCoroutine(ApplySlowForSeconds(movement));
    }

    private IEnumerator ApplySlowForSeconds(MovementScript movement)
    {
        movement.SetSlowed(true);

        float originalSpeed = movement.speed;
        float originalDashSpeed = movement.dashSpeed;

        movement.speed = originalSpeed * slowMultiplier;
        movement.dashSpeed = originalDashSpeed * slowMultiplier;

        yield return new WaitForSeconds(slowDuration);

        if(movement != null) 
        {
            movement.speed = originalSpeed;
            movement.dashSpeed = originalDashSpeed;
            movement.SetSlowed(false);
        }
        Destroy(gameObject);
    }

    private IEnumerator DestroyIfNoHit() //made this so icicle destorys if it doesnt hit anyhting 
    {
        yield return new WaitForSeconds(lifetime);
        if (!hasHit)
        {
            Destroy(gameObject);
        }
    }
}