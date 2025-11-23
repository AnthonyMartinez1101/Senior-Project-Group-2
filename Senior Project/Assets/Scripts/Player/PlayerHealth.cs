using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 20f;
    public float currentHealth;

    private FloatingHealth healthBar;

    private DamageFlash damageFlash;

    [SerializeField] private ShopScript shop;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        if (!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.SetMax();

        damageFlash = GetComponent<DamageFlash>();

        if (shop == null)
        {
            Debug.Log("PlayerHealth: Shop not assigned in inspector, searching for ShopScript.");
            shop = FindObjectOfType<ShopScript>();
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


    public bool IsMaxHealth()
    {
        return currentHealth >= maxHealth;
    }
}
