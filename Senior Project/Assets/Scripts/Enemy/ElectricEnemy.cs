using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ElectricZombie2D : MonoBehaviour
{
    public float linkRange = 6f; // Max distance to connect electricity
    public float damagePerSecond = 10f; // DPS when player touches arc
    public LineRenderer linePrefab; // 2D LineRenderer prefab

    private static List<ElectricZombie2D> allZombies = new List<ElectricZombie2D>();
    private Dictionary<ElectricZombie2D, LineRenderer> activeLinks = new Dictionary<ElectricZombie2D, LineRenderer>();

    void OnEnable()
    {
        allZombies.Add(this);
    }

    void OnDisable()
    {
        // Clear links and notify other zombies before removing from list
        ClearLinks();
        allZombies.Remove(this);
    }

    void Update()
    {
        UpdateLinks();
    }

    void UpdateLinks()
    {
        // Iterate over a copy to avoid issues if list is modified during iteration
        foreach (var other in allZombies.ToArray())
        {
            if (other == this) continue;

            float dist = Vector2.Distance(transform.position, other.transform.position);

            if (dist <= linkRange)
            {
                // Only create a single shared link per pair. Designate the zombie with the lower instance ID as the owner/creator.
                bool linkExists = activeLinks.ContainsKey(other) || other.activeLinks.ContainsKey(this);
                if (!linkExists && GetInstanceID() < other.GetInstanceID())
                {
                    CreateLink(other);
                }
            }
            else
            {
                // If this side owns the link entry, remove it. If the other side owns it, the owner will remove it in its own Update.
                if (activeLinks.ContainsKey(other))
                {
                    RemoveLink(other);
                }
            }
        }

        // Update line positions for links this zombie knows about
        foreach (var pair in activeLinks)
        {
            if (pair.Key == null || pair.Value == null) continue;
            pair.Value.SetPosition(0, transform.position);
            pair.Value.SetPosition(1, pair.Key.transform.position);
        }
    }

    void CreateLink(ElectricZombie2D other)
    {
        // Instantiate the line renderer and configure it
        LineRenderer lr = Instantiate(linePrefab);
        lr.positionCount = 2;
        lr.useWorldSpace = true;

        // Store the same LineRenderer reference on both zombies so it represents one shared arc
        activeLinks[other] = lr;
        other.activeLinks[this] = lr;

        // Add damage handler to the line object
        ElectricArcDamage2D dmg = lr.gameObject.AddComponent<ElectricArcDamage2D>();
        dmg.zombieA = this;
        dmg.zombieB = other;
        dmg.damagePerSecond = damagePerSecond;

        // Initialize positions immediately
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, other.transform.position);
    }

    void RemoveLink(ElectricZombie2D other)
    {
        if (!activeLinks.ContainsKey(other)) return;

        LineRenderer lr = activeLinks[other];

        // Only the owner (the creator) should destroy the shared LineRenderer to avoid double-destroy
        if (GetInstanceID() < other.GetInstanceID())
        {
            if (lr != null)
                Destroy(lr.gameObject);
        }

        activeLinks.Remove(other);

        // Remove the back-reference on the other zombie if present
        if (other != null && other.activeLinks.ContainsKey(this))
        {
            other.activeLinks.Remove(this);
        }
    }

    void ClearLinks()
    {
        // Iterate a copy because we will modify dictionaries while iterating
        foreach (var kv in activeLinks.ToList())
        {
            var other = kv.Key;
            var lr = kv.Value;

            // Owner destroys the shared GameObject
            if (other != null && GetInstanceID() < other.GetInstanceID())
            {
                if (lr != null)
                    Destroy(lr.gameObject);
            }

            // Remove the back-reference
            if (other != null && other.activeLinks.ContainsKey(this))
            {
                other.activeLinks.Remove(this);
            }
        }

        activeLinks.Clear();
    }
}

// Handles player damage when touching electricity (2D)
public class ElectricArcDamage2D : MonoBehaviour
{
    public ElectricZombie2D zombieA;
    public ElectricZombie2D zombieB;
    public float damagePerSecond;

    private CapsuleCollider2D col;

    void Start()
    {
        col = gameObject.AddComponent<CapsuleCollider2D>();
        col.isTrigger = true;
        col.direction = CapsuleDirection2D.Horizontal;
    }

    void Update()
    {
        if (zombieA == null || zombieB == null) return;

        Vector2 a = zombieA.transform.position;
        Vector2 b = zombieB.transform.position;

        Vector2 mid = (a + b) / 2f;
        transform.position = mid;

        Vector2 dir = b - a;
        float dist = dir.magnitude;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Thin collider along the arc
        col.size = new Vector2(dist, 0.25f);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }
    }
}
