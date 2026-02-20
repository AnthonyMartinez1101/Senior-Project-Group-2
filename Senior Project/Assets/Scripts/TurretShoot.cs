using UnityEngine;

public class TurretShoot : MonoBehaviour
{
    public GameObject turretShootPrefab;

    private Transform currentTarget;

    public float shootCooldown = 0.5f;

    // Update is called once per frame
    void Update()
    {
        shootCooldown -= Time.deltaTime;
        if (currentTarget != null && shootCooldown <= 0f)
        {
            if (turretShootPrefab == null)
            {
                Debug.LogWarning("TurretShoot: No turret shoot prefab assigned.");
                return;
            }

            Instantiate(turretShootPrefab, transform.position, Quaternion.identity);
            shootCooldown = 0.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Boss") || collision.CompareTag("Enemy"))
        {
            currentTarget = collision.transform;
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss") || collision.CompareTag("Enemy"))
        {
            if (currentTarget == collision.transform)
            {
                currentTarget = null;
            }
        }
    }   
}
