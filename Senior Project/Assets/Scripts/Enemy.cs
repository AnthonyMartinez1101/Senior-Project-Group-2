using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 5f;
    private FloatingHealth healthBar;

    private EnemySpawner spawner; //Temp for now to determine Day and Night (WILL REPLACE WITH WORLD CLOCK)

    private void Start()
    {
        health = maxHealth;

        if(!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.SetMax();
    }

    public void SetSpawner(EnemySpawner newSpawner)
    {
        spawner = newSpawner;
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if(healthBar) healthBar.UpdateHealth(health, maxHealth);
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawner.IsDay())
        {
            TakeDamage(0.04f);
        }
    }
}
