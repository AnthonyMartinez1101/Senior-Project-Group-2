using UnityEngine;
using System.Collections.Generic;

public class ElectricZombie2D : MonoBehaviour
{
    public float linkRange = 3f;
    public float damagePerSecond = 5f;
    public LineRenderer linePrefab;
    public int maxLinks = 3;

    private static List<ElectricZombie2D> allZombies = new List<ElectricZombie2D>();

    // Simple list storing pairs of other zombie and the shared LineRenderer
    class LinkEntry
    {
        public ElectricZombie2D other;
        public LineRenderer lr;
    }

    private List<LinkEntry> activeLinks = new List<LinkEntry>();

    // Circle trigger used to detect nearby zombies
    private CircleCollider2D rangeTrigger;

    void OnValidate()
    {
        // Keep trigger radius in sync in editor when changed
        if (rangeTrigger == null)
            rangeTrigger = GetComponent<CircleCollider2D>();
        if (rangeTrigger != null)
            rangeTrigger.radius = linkRange;
    }

    void Awake()
    {
        // Ensure there is a Rigidbody2D so triggers generate events between zombies
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic; // keeps physics stable but allows trigger callbacks
        }
        else
        {
            // If present, prefer kinematic to avoid unwanted physics pushes
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Ensure there is a CircleCollider2D set as trigger for proximity detection
        rangeTrigger = GetComponent<CircleCollider2D>();
        if (rangeTrigger == null)
        {
            rangeTrigger = gameObject.AddComponent<CircleCollider2D>();
            rangeTrigger.isTrigger = true;
        }
        else
        {
            rangeTrigger.isTrigger = true;
        }

        rangeTrigger.radius = linkRange;
    }

    void OnEnable()
    {
        allZombies.Add(this);
    }

    void OnDisable()
    {
        // Remove all links we own/destroyed and clear list
        ClearLinks();
        allZombies.Remove(this);
    }

    void Update()
    {
        // Keep radius in sync in case linkRange changed at runtime
        if (rangeTrigger != null && rangeTrigger.radius != linkRange)
            rangeTrigger.radius = linkRange;

        UpdateLinks();
    }

    void OnTriggerEnter2D(Collider2D otherCol)
    {
        // Called when another zombie enters our circle trigger
        ElectricZombie2D other = otherCol.GetComponent<ElectricZombie2D>();
        if (other == null || other == this) return;

        // Only one of the two zombies should create the shared link to avoid duplicates
        if (GetInstanceID() < other.GetInstanceID())
        {
            // Check not already linked and both sides can accept a new link
            if (!HasLinkWith(other) && CanAcceptLink() && other.CanAcceptLink())
            {
                CreateLink(other);
            }
        }
    }

    void OnTriggerExit2D(Collider2D otherCol)
    {
        ElectricZombie2D other = otherCol.GetComponent<ElectricZombie2D>();
        if (other == null || other == this) return;

        // If we own the link entry remove it
        if (GetInstanceID() < other.GetInstanceID())
        {
            RemoveLink(other);
        }
    }

    void UpdateLinks()
    {
        // Update positions of each active link and prune invalid ones
        for (int i = activeLinks.Count - 1; i >= 0; i--)
        {
            var entry = activeLinks[i];
            if (entry == null || entry.other == null || entry.lr == null)
            {
                // Clean up
                if (entry != null && entry.lr != null)
                    Destroy(entry.lr.gameObject);
                activeLinks.RemoveAt(i);
                continue;
            }

            // If other moved out of range (in case linkRange was changed dynamically), remove link
            float dist = Vector2.Distance(transform.position, entry.other.transform.position);
            if (dist > linkRange)
            {
                RemoveLink(entry.other);
                continue;
            }

            // Update line positions
            entry.lr.SetPosition(0, transform.position);
            entry.lr.SetPosition(1, entry.other.transform.position);
        }
    }

    bool HasLinkWith(ElectricZombie2D other)
    {
        for (int i = 0; i < activeLinks.Count; i++)
        {
            if (activeLinks[i].other == other) return true;
        }
        return false;
    }

    // Counts all links for this zombie (owned + links owned by others that point to this)
    int GetTotalConnections()
    {
        int count = activeLinks.Count;
        for (int i = 0; i < allZombies.Count; i++)
        {
            var z = allZombies[i];
            if (z == null || z == this) continue;
            if (z.HasLinkWith(this)) count++;
        }
        return count;
    }

    // Public helper so other instances can check if they can accept a new owned link
    public bool CanAcceptLink()
    {
        return GetTotalConnections() < maxLinks;
    }

    void CreateLink(ElectricZombie2D other)
    {
        // Double-check limits before creating
        if (!CanAcceptLink())
        {
            Debug.LogWarning($"ElectricZombie2D: cannot create link, this zombie already has maxLinks ({maxLinks}).");
            return;
        }
        if (!other.CanAcceptLink())
        {
            Debug.LogWarning($"ElectricZombie2D: cannot create link, other zombie reached its maxLinks ({other.maxLinks}).");
            return;
        }

        if (linePrefab == null)
        {
            Debug.LogWarning("ElectricZombie2D: linePrefab not set. Cannot create visual arc.");
            return;
        }

        // Compute midpoint & rotation so we can place the instantiated object correctly immediately
        Vector3 a = transform.position;
        Vector3 b = other.transform.position;
        Vector3 mid = (a + b) / 2f;
        Vector3 dir = b - a;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Instantiate the line renderer GameObject at the correct position/rotation to avoid transient collider overlap at origin
        LineRenderer lr = Instantiate(linePrefab, mid, Quaternion.Euler(0f, 0f, angle));
        lr.positionCount = 2;
        lr.useWorldSpace = true;

        // Immediately disable any colliders on the instantiated object (and children) to prevent transient overlap
        var allColliders = lr.gameObject.GetComponentsInChildren<Collider2D>(true);
        for (int i = 0; i < allColliders.Length; i++)
        {
            allColliders[i].enabled = false;
        }

        // Basic default widths if none set on prefab
        if (lr.startWidth <= 0f) lr.startWidth = 0.05f;
        if (lr.endWidth <= 0f) lr.endWidth = 0.05f;

        // Ensure it's rendered above sprites by default (adjust as needed)
        var renderer = lr.GetComponent<Renderer>();
        if (renderer != null)
        {
            try { renderer.sortingLayerName = "Default"; } catch { }
            renderer.sortingOrder = 5;
        }

        // Parent the line to a neutral root to keep hierarchy clean
        lr.transform.SetParent(null);

        // Add damage behaviour (adds its own trigger collider)
        ElectricArcDamage2D dmg = lr.gameObject.GetComponent<ElectricArcDamage2D>();
        if (dmg == null) dmg = lr.gameObject.AddComponent<ElectricArcDamage2D>();

        // Set references and DPS
        dmg.zombieA = this;
        dmg.zombieB = other;
        dmg.damagePerSecond = damagePerSecond;

        // Force the visual points now (keeps line correct immediately)
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, other.transform.position);

        // Store link entry on our side. The other zombie will not create a duplicate because of the instance ID rule.
        activeLinks.Add(new LinkEntry { other = other, lr = lr });
    }

    void RemoveLink(ElectricZombie2D other)
    {
        for (int i = activeLinks.Count - 1; i >= 0; i--)
        {
            if (activeLinks[i].other == other)
            {
                var lr = activeLinks[i].lr;
                if (lr != null)
                {
                    Destroy(lr.gameObject);
                }
                activeLinks.RemoveAt(i);
            }
        }
    }

    void ClearLinks()
    {
        for (int i = 0; i < activeLinks.Count; i++)
        {
            var entry = activeLinks[i];
            if (entry != null && entry.lr != null)
                Destroy(entry.lr.gameObject);
        }
        activeLinks.Clear();
    }

    // Draw the link range in editor for convenience
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan * 0.6f;
        Gizmos.DrawWireSphere(transform.position, linkRange);
    }
}

// Handles player damage when touching electricity (2D)
public class ElectricArcDamage2D : MonoBehaviour
{
    public ElectricZombie2D zombieA;
    public ElectricZombie2D zombieB;    
    public float damagePerSecond;

    private CapsuleCollider2D col;
    private bool hasInitialized = false;

    // small distance tolerance to avoid floating point / timing false positives
    private const float DistanceEpsilon = 0.02f;

    void Start()
    {
        // Ensure a CapsuleCollider2D exists and is a trigger. Keep it disabled so we don't rely on trigger callbacks.
        col = gameObject.GetComponent<CapsuleCollider2D>();
        if (col == null) col = gameObject.AddComponent<CapsuleCollider2D>();
        col.isTrigger = true;
        col.direction = CapsuleDirection2D.Horizontal;
        col.offset = Vector2.zero;

        // Keep collider disabled (we use Physics2D.OverlapCapsuleAll + distance check instead)
        col.enabled = false;
    }

    void Update()
    {
        if (zombieA == null || zombieB == null)
            return;

        Vector2 a = zombieA.transform.position;
        Vector2 b = zombieB.transform.position;

        Vector2 mid = (a + b) / 2f;
        transform.position = mid;

        Vector2 dir = b - a;
        float dist = dir.magnitude;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Thin collider along the arc (used only for sizing; stays disabled)
        if (col != null)
        {
            col.size = new Vector2(dist, 0.25f);

            // mark initialized so we start overlap checks (but do NOT enable the trigger collider)
            if (!hasInitialized)
            {
                hasInitialized = true;
            }
        }

        // After initialization, explicitly test capsule overlap with physics query (no trigger callbacks)
        if (hasInitialized)
        {
            // Query physics for colliders overlapping the capsule area
            Collider2D[] hits = Physics2D.OverlapCapsuleAll(mid, col.size, col.direction, angle);

            if (hits != null && hits.Length > 0)
            {
                var damagedPlayers = new HashSet<GameObject>();

                // half thickness of the capsule (used for precise geometric test)
                float halfThickness = 0.125f;
                if (col != null)
                {
                    halfThickness = Mathf.Abs(col.size.y) * 0.5f;
                    if (halfThickness <= 0f) halfThickness = 0.125f;
                }

                for (int i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];
                    if (hit == null) continue;
                    if (!hit.CompareTag("Player")) continue;

                    GameObject playerObj = hit.gameObject;
                    if (damagedPlayers.Contains(playerObj)) continue;

                    // Use player's transform position for a robust point-to-segment test
                    Vector2 playerPos = playerObj.transform.position;
                    float distanceToSegment = DistancePointToSegment(playerPos, a, b);

                    // Only apply damage if the player's position is actually within the visible arc thickness
                    if (distanceToSegment <= halfThickness + DistanceEpsilon)
                    {
                        damagedPlayers.Add(playerObj);
                        PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
                        if (health != null)
                        {
                            health.TakeDamage(damagePerSecond * Time.deltaTime);
                        }
                    }
                }
            }
        }
    }

    // helper: distance from point p to segment a-b
    static float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float ab2 = Vector2.Dot(ab, ab);
        if (ab2 == 0f) return Vector2.Distance(p, a);
        float t = Vector2.Dot(p - a, ab) / ab2;
        t = Mathf.Clamp01(t);
        Vector2 proj = a + t * ab;
        return Vector2.Distance(p, proj);
    }

    void OnDisable()
    {
        // Keep collider disabled when the object is destroyed/disabled
        if (col != null)
            col.enabled = false;
    }
}
