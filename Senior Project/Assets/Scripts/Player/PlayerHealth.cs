using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 20f;
    public float currentHealth;
    public float startHealth = 20f;

    private FloatingHealth healthBar;

    private DamageFlash damageFlash;

    [SerializeField] private ShopScript shop;

    public bool isInvincible = false;

    // Diagnostics / debounce
    private float lastDamageTime = -10f;
    private int damageLogCount = 0;
    private const float SmallDamageThreshold = 0.01f; // treat smaller spikes as noise if frequent
    private const float SmallDamageDebounce = 0.15f;  // time window to suppress tiny repeat damage

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = startHealth;
        if (!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();

        if (healthBar && currentHealth == maxHealth) healthBar.SetMax();
        else healthBar.UpdateHealth(currentHealth, maxHealth);

        damageFlash = GetComponent<DamageFlash>();

        if (shop == null)
        {
            Debug.Log("PlayerHealth: Shop not assigned in inspector");
        }
    }

    public void Heal(float healAmount)
    {
        if (damageFlash) damageFlash.FlashOnHeal();
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (healthBar) healthBar.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        // Defensive: ignore invalid/zero/negative damage
        if (isInvincible) return;
        if (float.IsNaN(damageAmount) || float.IsInfinity(damageAmount)) return;
        if (damageAmount <= 0f) return;

        // Debounce tiny damage spikes that might be artifacts of physics timing
        float now = Time.time;
        if (damageAmount < SmallDamageThreshold && now - lastDamageTime < SmallDamageDebounce)
        {
            // treat as noise — ignore
            return;
        }

        lastDamageTime = now;

        // Log first few occurrences so we can trace where damage originates
        if (damageLogCount < 5)
        {
            damageLogCount++;
            Debug.Log($"PlayerHealth.TakeDamage called: amount={damageAmount:F4}\nStackTrace:\n{new System.Diagnostics.StackTrace(1, true)}");
        }

        shop.CloseShop();
        GameManager.Instance.CameraShake(damageAmount + 3f, 0.2f);
        if (damageFlash) damageFlash.FlashOnDamage();
        currentHealth -= damageAmount;
        if (healthBar) healthBar.UpdateHealth(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            GameManager.Instance.RestartScene();
            Destroy(gameObject);
        }
    }

    public void SetHealth(float newHealth)
    {
        currentHealth = newHealth;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (healthBar) healthBar.UpdateHealth(currentHealth, maxHealth);
    }

    public bool IsMaxHealth()
    {
        return currentHealth >= maxHealth;
    }

    public void SetMaxHealth()
    {
        currentHealth = maxHealth;
        if (healthBar) healthBar.UpdateHealth(currentHealth, maxHealth);
    }
}
