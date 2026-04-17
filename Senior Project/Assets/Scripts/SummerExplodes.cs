using UnityEngine;
using System.Collections;

public class SummerExplodes : MonoBehaviour
{
    public Collider2D explosionRadius;
    [SerializeField] private int damage = 3;
    [SerializeField] private float lifetime = 0.25f;
    [SerializeField] private float delay = 1f;

    private CircleCollider2D col;
    private bool hitPlayer;

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

        Destroy(gameObject);
    }

    //void Start()
    //{
    //    Destroy(gameObject, lifetime);
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(hitPlayer) return;
        if(!collision.CompareTag("Player")) return;

        hitPlayer = true;

        var health = collision.GetComponent<PlayerHealth>();
        if(health != null)
        {
            health.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
