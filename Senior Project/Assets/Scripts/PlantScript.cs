using System.Collections;
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

    private bool tutorialMode = false;

    private SoilScript connectedSoil;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Create(PlantItem newPlantInfo, SoilScript newSoil)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        plantInfo = newPlantInfo;
        connectedSoil = newSoil;

        spriteRenderer.sprite = plantInfo.growStages[0];

        sparkles = gameObject.transform.GetChild(0).gameObject;
        sparkles.SetActive(false);

        currentHealth = plantHealth;

        if (!healthBar) healthBar = GetComponentInChildren<FloatingHealth>();
        if (healthBar) healthBar.SetMax();

        if(newPlantInfo.plantName == "Tutorial Plant") tutorialMode = true;

        StartCoroutine(TempColliderDis());
    }

    IEnumerator TempColliderDis()
    { 
        var col = GetComponent<Collider2D>();
        col.enabled = false;
        yield return new WaitForSeconds(0.2f);
        col.enabled = true;
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
        if (tutorialMode && !IsFullyGrown()) return;

        currentHealth -= damageAmount * 3f;
        if (healthBar) healthBar.UpdateHealth(currentHealth, plantHealth);
        if (currentHealth <= 0)
        {
            if (lastCollidedWithSickle)
            {
                if (IsFullyGrown())
                {
                    if(plantInfo.produce != null) DropProduce();
                    else Debug.Log("PlantScript: PlantItem produce is null. Cannot drop produce.");

                    if(plantInfo.objectOnHarvest != null)
                    {
                        var newObject = Instantiate(plantInfo.objectOnHarvest, transform.position, Quaternion.identity);

                        var newPlant = newObject.GetComponent<PlantScript>();
                        if (newPlant != null) connectedSoil.NewPlant(newPlant);
                    }
                }
                else
                {
                    if(plantInfo.seed != null) DropSeed();
                    else Debug.Log("PlantScript: PlantItem seed is null. Cannot drop seed.");
                }
            }
            Destroy(gameObject);
        }
    }

    private void DropProduce()
    {
        //Default amount to drop
        int dropAmount = 1;

        //Get range of produce drops and set drop amount to random value
        int min = plantInfo.minDrop;
        int max = plantInfo.maxDrop;
        if (min <= max) dropAmount = Random.Range(min, max + 1);

        for(int i = 0; i < dropAmount; i++)
        {
            bool expires = !tutorialMode;
            ItemDropFactory.Instance.SpawnItem(plantInfo.produce, 0, transform.position, expires);
        }

        TryToDropSeed();
    }


    private void TryToDropSeed()
    {
        int seedDropChance = plantInfo.seedChance;
        int rand = Random.Range(1, 101); //Random number between 1 and 100
        if (rand <= seedDropChance)
        {
            DropSeed();
        }
    }

    private void DropSeed()
    {
        ItemDropFactory.Instance.SpawnItem(plantInfo.seed, 0, transform.position, expires: true);
    }

    public void TakeWaterDamage(float damageAmount)
    {
        if (plantInfo.canDry)
        {
            currentHealth -= damageAmount;
            if (healthBar) healthBar.UpdateHealth(currentHealth, plantHealth);
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public bool IsFullyGrown()
    {
        return currentGrowth >= plantInfo.growthTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(CheckCollision(collision)) lastCollidedWithSickle = collision.CompareTag("Sickle");
    }

    //Check collision if it doesn't interact with the following
    private bool CheckCollision(Collider2D collision)
    {
        return (!collision.CompareTag("Player") 
            && !collision.CompareTag("Interact")
            && !collision.CompareTag("NoBulletCollision"));
    }
}
