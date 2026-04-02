using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 20f;
    public float currentHealth;
    public float startHealth = 20f;
    [SerializeField] private float maxHealthBuffPercentage = 0f;
    [SerializeField] private float actualMaxHealth = 0f;

    private FloatingHealth healthBar;

    private DamageFlash damageFlash;

    [SerializeField] private ShopScript shop;

    public bool isInvincible = false;
    int poisonCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = startHealth;
        if (!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();

        if (healthBar && currentHealth == actualMaxHealth) healthBar.SetMax();
        else if (healthBar) healthBar.UpdateHealth(currentHealth, actualMaxHealth);

        damageFlash = GetComponent<DamageFlash>();

        if (shop == null)
        {
            Debug.Log("PlayerHealth: Shop not assigned in inspector");
        }
    }

    void Update()
    {
        actualMaxHealth = maxHealth * (1f + maxHealthBuffPercentage / 100);
        if(healthBar) healthBar.UpdateHealth(currentHealth, actualMaxHealth); 
    }

    public void Heal(float healAmount)
    {
        if(healAmount <= 0f) return;

        if (damageFlash) damageFlash.FlashOnHeal();

        currentHealth += healAmount;
        if (currentHealth > actualMaxHealth) currentHealth = actualMaxHealth;
        if (healthBar) healthBar.UpdateHealth(currentHealth, actualMaxHealth);
    }

    public void HealthBuff()
    {
        maxHealthBuffPercentage += 0.5f;
        StatManager.Instance.AddHealthBuff(0.5f);
    }
    public void TakeDamage(float damageAmount)
    {
        // Defensive: ignore invalid/zero/negative damage
        if (isInvincible) return;
        if (float.IsNaN(damageAmount) || float.IsInfinity(damageAmount)) return;
        if (damageAmount <= 0f) return;

        // Apply health change
        currentHealth -= damageAmount;
        StatManager.Instance.AddDamageTaken(damageAmount);

        //Close shop if opened when taking damage
        shop.CloseShop();
        GameManager.Instance.CameraShake(damageAmount + 3f, 0.2f);
        if (damageFlash) damageFlash.FlashOnDamage();

        if (healthBar) healthBar.UpdateHealth(currentHealth, actualMaxHealth);

        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOverScene();
            Destroy(gameObject);
        }
    }

    public void SetHealth(float newHealth)
    {
        currentHealth = newHealth;
        if (currentHealth > actualMaxHealth) currentHealth = actualMaxHealth;
        if (healthBar) healthBar.UpdateHealth(currentHealth, actualMaxHealth);
    }

    public bool IsMaxHealth()
    {
        return currentHealth >= actualMaxHealth;
    }

    public void SetMaxHealth()
    {
        currentHealth = actualMaxHealth;
        if (healthBar) healthBar.UpdateHealth(currentHealth, actualMaxHealth);
    }

    public void ApplyPoison(int ticks)
    {
        bool ifPoisoned = poisonCount > 0;

        poisonCount = ticks;

        if (!ifPoisoned) StartCoroutine(PoisonDamage());
    }

    IEnumerator PoisonDamage()
    {
        while (poisonCount > 0)
        {
            TakeDamage(1f);
            poisonCount--;
            yield return new WaitForSeconds(0.75f);
        }
    }
}