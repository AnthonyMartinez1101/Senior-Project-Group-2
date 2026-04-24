using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class SoilScript : MonoBehaviour, ICheat
{
    private GameObject highlighted; //Highlight child object
    public PlantScript plantActor; //Plant Actor prefab 

    public PlantScript currentPlant; //Current plant instance

    public float waterLevel = 0f;

    private GameObject waterDroplet; //Child object in soil representing water droplet

    public WorldClock worldClock;

    private SpriteRenderer sr;
    public Color dryColor;
    public Color wetColor;

    public GameObject waterSplash;

    private bool cheatMode = false;


    //private InteractScript interactScript;

    private void Start()
    {
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        highlighted = transform.GetChild(1).gameObject;
        highlighted.SetActive(false);

        waterDroplet = transform.GetChild(2).gameObject;
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
        //Water logic
        LightenSoil();
        waterLevel -= Time.deltaTime;

        bool isDay = worldClock.IsDay();

        //Growth logic
        GrowthLogic(isDay);

        //Water droplet Logic
        WaterDropletLogic(isDay);
    }

    public void SetCheats()
    {
        cheatMode = true;
    }

    private void GrowthLogic(bool isDay)
    {
        if (!currentPlant) return;

        if (cheatMode)
        {
            currentPlant.TickGrowth(Time.deltaTime * 60, true);
            return;
        }

        
        if (isWatered())
        {
            currentPlant.TickGrowth(Time.deltaTime, isDay);
        }
        else if (isDay)
        {
            currentPlant.TakeWaterDamage(Time.deltaTime);
        }
    }

    private void WaterDropletLogic(bool isDay)
    {
        if (!currentPlant || currentPlant.IsFullyGrown())
        {
            waterDroplet.SetActive(false);
            return;
        }

        if (currentPlant.CanDry() && isDay)
        {
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
            if (isWatered())
            {
                waterDroplet.SetActive(false);
            }
            else if (currentPlant.IsDamaged())
            {
                waterDroplet.SetActive(true);
                BlinkDroplet();
            }
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
        
        sr.color = wetColor;
    }

    private void LightenSoil()
    {
        float t = Mathf.Clamp01(waterLevel / 10f); // Normalize water level to [0,1]
        Color blendedColor = Color.Lerp(dryColor, wetColor, t);
        sr.color = blendedColor;
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
        if (waterSplash) Destroy(Instantiate(waterSplash, transform.position, Quaternion.identity), 0.5f); //Spawn splash, destroy after 0.5 secs

        if (currentPlant == null) return;
        SpinkleWater sprinkler = currentPlant.GetComponent<SpinkleWater>();

        if(sprinkler == null) return;
        sprinkler.FillWater();
    }

    public bool Plant(Item item)
    {
        var seedData = item.extraItemData as SeedData;
        if (seedData == null) return false; //Invalid seed data 

        if (currentPlant != null) return false; //Already planted
        if (item == null || item.itemType != ItemType.Seed || seedData.plant == null) return false; //Invalid plant item 

        currentPlant = Instantiate(plantActor, transform.position, Quaternion.identity, transform);
        currentPlant.Create(seedData.plant, this);

        return true;
    }

    public void NewPlant(PlantScript newPlantItem)
    {
        currentPlant = newPlantItem;
        currentPlant.Create(newPlantItem.plantInfo, this);
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