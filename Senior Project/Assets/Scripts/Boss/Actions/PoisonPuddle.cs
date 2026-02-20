using UnityEngine;

public class PoisonPuddle : MonoBehaviour
{
    public float duration = 5f;
    public float damage = 2f;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D player)
    {
        if (player.CompareTag("Player"))
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage * Time.deltaTime);
            }
        }
    }
}
