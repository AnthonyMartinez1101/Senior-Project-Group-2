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

            //Instantiate(turretShootPrefab, transform.position, Quaternion.identity); removing for now sdince quartenion.identity wont rotate it

            Vector2 direction = (currentTarget.position - transform.position).normalized; // angle in degrees

            Vector2 spawnPosition = (Vector2)transform.position + (direction * 0.5f); // spawn position slightly in front of the turret

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // rotaiton based on the angle

            Quaternion rotation = Quaternion.Euler(0, 0, angle + 90f); //weird offset 

            Instantiate(turretShootPrefab, spawnPosition, rotation);


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
