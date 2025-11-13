using UnityEngine;

public class PlantScript : MonoBehaviour
{
    public PlantItem plantInfo;

    public float currentGrowth = 0f;

    private SpriteRenderer spriteRenderer;

    private GameObject sparkles;

    private float plantHealth = 20f; 
    public float currentHealth;
    private FloatingHealth healthBar;

    private bool lastCollidedWithSickle = false;




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
                spriteRenderer.sprite = plantInfo.growStages[3];
                sparkles.SetActive(true);
            }
            else if (growthPercent >= 0.66f)
            {
                spriteRenderer.sprite = plantInfo.growStages[2];
            }
            else if (growthPercent >= 0.33f)
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
        currentHealth -= damageAmount * 3f;
        if (healthBar) healthBar.UpdateHealth(currentHealth, plantHealth);
        if (currentHealth <= 0)
        {
            if (lastCollidedWithSickle)
            {
                if(plantInfo.produce != null && plantInfo.seed != null)
                {
                    ItemDropFactory.Instance.SpawnItem(plantInfo.seed, transform.position);
                    if (IsFullyGrown())
                    {
                        ItemDropFactory.Instance.SpawnItem(plantInfo.produce, transform.position);

                        //25% chance to get an extra seed when harvesting fully grown crop
                        int rand = Random.Range(0, 4);
                        if(rand == 0)
                        {
                            ItemDropFactory.Instance.SpawnItem(plantInfo.seed, transform.position);
                        }
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
        if(!collision.CompareTag("Player")) lastCollidedWithSickle = collision.CompareTag("Sickle");
    }
}
