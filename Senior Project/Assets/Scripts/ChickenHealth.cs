using UnityEngine;
using System.Collections;

public class ChickenHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 10f;
    private float currentHealth;

    private ChickenWander chickenWander;
    public GameObject chickenExplosion;

    private FloatingHealth healthBar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chickenWander = GetComponent<ChickenWander>();
        healthBar = GetComponentInChildren<FloatingHealth>();
        currentHealth = maxHealth;
        if (healthBar) healthBar.SetMax();

        StartCoroutine(TempColliderDis());
    }

    // Update is called once per frame
    void Update()
    {
        Heal();
        if(healthBar) healthBar.UpdateHealth(currentHealth, maxHealth);
    }

    IEnumerator TempColliderDis()
    {
        var col = GetComponent<Collider2D>();
        col.enabled = false;
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
    }

    private void Heal()
    {
        currentHealth += Time.deltaTime / 5f;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float damageDealt, DamageType damageType)
    {
        currentHealth -= damageDealt;
        if (currentHealth <= 0) Die();
        else if(chickenWander) chickenWander.StartFleeing();
    }

    private void Die()
    {
        //Spawn explosion
        if (chickenExplosion)
        {
            GameObject explosion = Instantiate(chickenExplosion, transform.position, Quaternion.identity);
            Destroy(explosion, 10f);
        }
        Destroy(gameObject);
    }
}
