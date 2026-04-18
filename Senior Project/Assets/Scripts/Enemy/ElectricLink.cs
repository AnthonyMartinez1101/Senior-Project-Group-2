using UnityEngine;

public class ElectricLink : MonoBehaviour
{
    [SerializeField] private float thickness = 0.1f;
    [SerializeField] private int dps = 1;
    [SerializeField] private float tickInt = 0.5f;

    public Transform A { get; private set; }
    public Transform B { get; private set; }

    private LineRenderer lr;
    private BoxCollider2D box;
    private PlayerHealth playerHealth;
    private float nextTickTime;

    public void Init(Transform a, Transform b)
    {
        A = a;
        B = b;
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        box = GetComponent<BoxCollider2D>();
        if(box != null) box.isTrigger = true;
    }

    private void LateUpdate()
    {
        if(A == null || B == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 a = A.position;
        Vector3 b = B.position;

        if(lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, a);
            lr.SetPosition(1, b);
        }
        if(box != null)
        {
            Vector2 mid = (a + b) * 0.5f;
            Vector2 dir = (b - a);
            float len = dir.magnitude;
            transform.position = mid;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            box.size = new Vector2(len, thickness);
            box.offset = Vector2.zero;
        }
        if(playerHealth != null && Time.time >= nextTickTime)
        {
            playerHealth.TakeDamage(dps);
            nextTickTime = Time.time + tickInt;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if(health == null) return;
        playerHealth = health;
        playerHealth.TakeDamage(dps);
        nextTickTime = Time.time + tickInt;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if(health == null) return;
        if(health == playerHealth)
        {
            playerHealth = null;
        }
    }
}