using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 20f;
    private float currentHealth;

    private FloatingHealth healthBar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        if (!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.SetMax();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (healthBar) healthBar.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (healthBar) healthBar.UpdateHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            GameManager.Instance.RestartScene();
            Destroy(gameObject);
        }
    }


    public bool IsMaxHealth()
    {
        return currentHealth >= maxHealth;
    }
}
