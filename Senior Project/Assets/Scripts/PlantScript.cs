using UnityEngine;

public class PlantScript : MonoBehaviour
{
    private PlantItem plantInfo;

    private float currentGrowth = 0f;

    private SpriteRenderer spriteRenderer;

    private GameObject sparkles;

    private float plantHealth = 20f; 
    private float currentHealth;
    private FloatingHealth healthBar;

    private Collider2D lastCollidedWith;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Create(PlantItem newPlantInfo)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        plantInfo = newPlantInfo;

        spriteRenderer.sprite = plantInfo.growStages[0];

        sparkles = gameObject.transform.GetChild(0).gameObject;
        sparkles.SetActive(false);

        currentHealth = plantHealth;

        if (!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.SetMax();
    }

    // Update is called once per frame
    public void TickGrowth(float dt)
    {
        if (currentHealth != plantHealth)
        {
            Heal(dt);
        }
        else
        {
            currentGrowth += dt;

            float growthPercent = currentGrowth / plantInfo.growthTime;

            if (growthPercent >= 1f)
            {
                spriteRenderer.sprite = plantInfo.growStages[2];
                sparkles.SetActive(true);
            }
            else if (growthPercent >= 0.5f)
            {
                spriteRenderer.sprite = plantInfo.growStages[1];
            }
        }
    }


    private void Heal(float dt)
    {
        //Heal at triple the speed of damage
        currentHealth += dt * 3;
        if (currentHealth > plantHealth) currentHealth = plantHealth;
        if (healthBar) healthBar.UpdateHealth(currentHealth, plantHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (healthBar) healthBar.UpdateHealth(currentHealth, plantHealth);
        if (currentHealth <= 0)
        {
            if (lastCollidedWith != null && lastCollidedWith.CompareTag("Sickle"))
            {
                if(plantInfo.produce != null && plantInfo.seed != null)
                {
                    GameManager.Instance.AddToInventory(plantInfo.seed);
                    if(IsFullyGrown())
                    {
                        GameManager.Instance.AddToInventory(plantInfo.produce);
                    }
                }
                else
                {
                    Debug.Log("PlantScript: PlantItem produce or seed is null. Cannot add to inventory.");
                } 
            }
            Destroy(gameObject);
        }
    }

    public void TakeWaterDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (healthBar) healthBar.UpdateHealth(currentHealth, plantHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public bool IsFullyGrown()
    {
        return currentGrowth >= plantInfo.growthTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        lastCollidedWith = collision;
    }
}
