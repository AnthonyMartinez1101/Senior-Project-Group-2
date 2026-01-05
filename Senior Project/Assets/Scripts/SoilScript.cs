using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SoilScript : MonoBehaviour
{
    private GameObject highlighted; //Highlight child object
    public PlantScript plantActor; //Plant Actor prefab

    public PlantScript currentPlant; //Current plant instance

    public float waterLevel = 0f;

    private GameObject waterDroplet; //Child object in soil representing water droplet

    public WorldClock worldClock;

    //private InteractScript interactScript;

    private void Start()
    {
        highlighted = transform.GetChild(0).gameObject;
        highlighted.SetActive(false);

        waterDroplet = transform.GetChild(1).gameObject;
        if (waterDroplet != null)
        {
            waterDroplet.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Water droplet object not found as child of soil.");
        }
    }
    private void Update()
    {
        if(worldClock.IsDay())
        {
            if (currentPlant != null)
            {
                if (!currentPlant.IsFullyGrown())
                {
                    //Growing logic
                    if (waterLevel > 0f)
                    {
                        currentPlant.TickGrowth(Time.deltaTime);
                    }
                    else if (waterLevel < 0f)
                    {
                        currentPlant.TakeWaterDamage(Time.deltaTime);
                    }

                    //Water droplet logic
                    if (waterLevel > 4f)
                    {
                        waterDroplet.SetActive(false);
                    }
                    else if (waterLevel > 0f)
                    {
                        waterDroplet.SetActive(true);
                        DropletFullAlpha();
                    }
                    else
                    {
                        waterDroplet.SetActive(true);
                        BlinkDroplet();
                    }
                }
                else 
                {                  
                    waterDroplet.SetActive(false);
                }
            }
            else
            {
                waterDroplet.SetActive(false);
            }
            LightenSoil();
            waterLevel -= Time.deltaTime;
        }
        else
        {
            waterDroplet.SetActive(false);
        }
    }

    private void DropletFullAlpha()
    {
        Color color = waterDroplet.GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        waterDroplet.GetComponent<SpriteRenderer>().color = color;
    }

    private void DarkenSoil()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        color.r = 0.5f;
        color.g = 0.25f;
        color.b = 0.1f;
        GetComponent<SpriteRenderer>().color = color;
    }

    private void LightenSoil()
    {
        float t = Mathf.Clamp01(waterLevel / 10f); // Normalize water level to [0,1]
        Color dryColor = new Color(1f, 1f, 1f); // Light dry soil color
        Color wetColor = new Color(0.4f, 0.2f, 0.1f); // Dark wet soil color
        Color blendedColor = Color.Lerp(dryColor, wetColor, t);
        GetComponent<SpriteRenderer>().color = blendedColor;
    }

    private void BlinkDroplet()
    {
        float blinkSpeed = 6f;
        float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; //Oscillates between 0 and 1
        Color color = waterDroplet.GetComponent<SpriteRenderer>().color;
        color.a = alpha;
        waterDroplet.GetComponent<SpriteRenderer>().color = color;
    }

    public void Water()
    {
        waterLevel = 10f;
        DarkenSoil();
    }

    public bool Plant(Item item)
    {
        var seedData = item.extraItemData as SeedData;
        if (seedData == null) return false; //Invalid seed data 

        if (currentPlant != null) return false; //Already planted
        if (item == null || item.itemType != ItemType.Seed || seedData.plant == null) return false; //Invalid plant item 

        currentPlant = Instantiate(plantActor, transform.position, Quaternion.identity, transform);
        currentPlant.Create(seedData.plant);

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interact"))
        {
            highlighted.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interact"))
        {
            highlighted.SetActive(false);
        }
    }

    public bool IsHighlighted()
    {
        return highlighted.activeSelf;
    }

    public bool isWatered()
    {
        return waterLevel > 0f;
    }
}

    //COMMENTED OUT FOR NEW IMPLEMENTATION
    //private float waterLevel = 0f;
    //private bool isPlanted = false;

    //public float plantGrowth = 0f;
    //public float maxPlantGrowth = 10f;

    //public float radius = 2f;
    //public Transform playerPos;

    //private FloatingHealth plantHealth;
    //public float maxHealth = 15f;
    //public float currentHealth = 15f;

    //public GameObject fullyWatered; //png of watered plant

    //InputAction waterButton;
    //InputAction plantButton;


    //void Start()
    //{
    //    waterButton = InputSystem.actions.FindAction("Water");
    //    plantButton = InputSystem.actions.FindAction("Plant");

    //    if (!plantHealth)
    //    {
    //        plantHealth = GetComponent<FloatingHealth>();
    //    }
    //    if (plantHealth)
    //    {
    //        plantHealth.SetMax();
    //    }
    //    if(fullyWatered)
    //    {
    //        fullyWatered.SetActive(false);
    //    }
    //}

    //void Update()
    //{
    //    float distance = Vector3.Distance(transform.position, playerPos.position);
    //    bool playerIsNear = distance <= radius;

    //    // Interacting logic
    //    if (playerIsNear)
    //    {
    //        // if watered
    //        if (waterButton.WasPressedThisFrame())
    //        {
    //            waterLevel = 10f;
    //            // set soil color to dark
    //        }
    //        if (plantButton.WasPressedThisFrame() && !isPlanted)
    //        {
    //            // if planted
    //            if (!isPlanted)
    //            {
    //                // remove seed from inventory
    //                isPlanted = true;
    //                plantGrowth = 0f;
                    
    //                if(fullyWatered)
    //                {
    //                    fullyWatered.SetActive(false);
    //                }
    //                // place plant sprite
    //            }
    //            // if harvested
    //            if (plantGrowth >= maxPlantGrowth && isPlanted)
    //            {
    //                resetSoil();
    //            }
    //        }
    //    }

    //    // Water diagetic health
    //    if (waterLevel > 0f)
    //    {
    //        waterLevel -= Time.deltaTime;
    //        // soil gets brighter
    //    }
    //    if (waterLevel <= 0f && isPlanted)
    //    {
    //        currentHealth -= Time.deltaTime;
    //        if (plantHealth)
    //        {
    //            plantHealth.UpdateHealth(currentHealth, maxHealth);
    //        }
    //    }

    //    // Plant growth logic
    //    if (isPlanted && waterLevel > 0f)
    //    {
    //        if (plantGrowth < maxPlantGrowth)
    //        {
    //            plantGrowth += Time.deltaTime;
    //            // increase plant size
    //        }
    //        if (plantGrowth >= maxPlantGrowth)
    //        {
    //            // show sparkles when done
    //            if(fullyWatered && fullyWatered == null)
    //            {
    //                fullyWatered = Instantiate(fullyWatered, transform.position, Quaternion.identity);
    //            }
    //        }
    //    }

    //    // Trampled logic
    //    // if (zombie touches soil && isPlanted)
    //    //{
    //    //    currentHealth -= 2f;
    //    //}

    //    // Plant death
    //    if (isPlanted && currentHealth <= 0f)
    //    {
    //        resetSoil();
    //    }
    //}

    //void resetSoil()
    //{
    //    isPlanted = false;
    //    plantGrowth = 0f;
    //    currentHealth = maxHealth;
    //    if (plantHealth)
    //    {
    //        plantHealth.SetMax();
    //    }
    //    if(fullyWatered)
    //    {
    //        fullyWatered.SetActive(false);
    //    }
    //    // reset soil color & plant
    //}
