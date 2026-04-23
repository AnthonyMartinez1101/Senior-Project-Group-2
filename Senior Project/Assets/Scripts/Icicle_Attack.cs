using UnityEngine;

public class Icicle_Attack : MonoBehaviour
{
    public float detectionRange = 8f;
    public float fireCooldown = 2f;
    public float projectileSpeed = 10f;

    public GameObject iciclePrefab;
    public Transform firePoint;

    float fireTimer = 2f;
    Transform Player;

    void Update()
    {
        if (fireTimer > 0f) fireTimer -= Time.deltaTime;

        if (Player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) Player = p.transform;
            return;
        }

        if (Vector2.Distance(transform.position, Player.position) < detectionRange)
            ShootAtPlayer();
    }

    void ShootAtPlayer()
    {
        if (fireTimer > 0f) return;
        Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;
        Vector2 dir = ((Vector2)Player.position - (Vector2)spawnPos).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle + 180);

        GameObject proj = Instantiate(iciclePrefab, spawnPos, rotation);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }
        fireTimer = fireCooldown;
    }
}