using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialScript : MonoBehaviour
{
    InputAction WASD;
    InputAction Space;

    InputAction scythe;

    public GameObject player;

    public EnemySpawner spawner;
    public int EnemyCount = 3;
    public Transform EnemyCollection;
    public WorldClock worldClock;

    public Transform GroundItemCollection;

    public Inventory inv;
    public Item seed;

    public SoilManager soilManager;

    public Item food;

    public ShopScript shop;

    public TutorialUI ui;

    private void Start()
    {
        WASD = InputSystem.actions.FindAction("Move");
        Space = InputSystem.actions.FindAction("Dash");

        scythe = InputSystem.actions.FindAction("Scythe");

        // Lock mechanics here
        player.GetComponent<InteractScript>().canPlant = false;
        player.GetComponent<InteractScript>().canWater = false;
        player.GetComponent<PlayerHealth>().isInvincible = true;
        player.GetComponent<PlayerHealth>().SetHealth(15f);
        CanCollectItems(false);
        shop.interactable = false;

        ui.UpdateUI("Use WASD to move around", false);

        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        Debug.Log("Starting tutorial");
        yield return StartCoroutine(MovementStage());
        Debug.Log("Movement complete");

        yield return StartCoroutine(DashStage());
        Debug.Log("Dash complete");

        yield return StartCoroutine(ScytheStage());
        Debug.Log("Scythe complete");

        yield return StartCoroutine(NightFightStage());
        Debug.Log("Night fight complete");

        yield return StartCoroutine(PickupItemsStage());
        Debug.Log("Pickup items complete");

        yield return StartCoroutine(PlantCropsStage());
        Debug.Log("Plant crops complete");

        yield return StartCoroutine(FillBucketStage());
        Debug.Log("Fill bucket complete");

        yield return StartCoroutine(WaterCropsStage());
        Debug.Log("Water crops complete");

        yield return StartCoroutine(HarvestCropsStage());
        Debug.Log("Harvest crops complete");

        yield return StartCoroutine(EatFoodStage());
        Debug.Log("Eat food complete");

        yield return StartCoroutine(SellItemsStage());
        Debug.Log("Sell items complete");

        ui.UpdateUI("Tutorial Complete!", true);
        Debug.Log("Tutorial complete, continuing to main game");

        yield return new WaitForSeconds(3f);
        // Tutorial complete, swap scenes (use fade to black?)
    }

    // Wait for WASD and Space to be pressed
    private IEnumerator MovementStage()
    {
        bool w = false, a = false, s = false, d = false;
        // Problem where each input needs to be pressed seperately (sorta)
        while (!(w && a && s && d))
        {
            Vector2 input = WASD.ReadValue<Vector2>();
            if (input.y > 0) w = true;
            if (input.y < 0) s = true;
            if (input.x < 0) a = true;
            if (input.x > 0) d = true;
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    private IEnumerator DashStage()
    {
        ui.UpdateUI("Use Space to dash while moving", false);

        bool dash = false;
        while (!dash)
        {
            Vector2 input = WASD.ReadValue<Vector2>();
            if(input != Vector2.zero && Space.WasPressedThisFrame())
            {
                dash = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    //Wait for scythe to be used
    private IEnumerator ScytheStage()
    {
        ui.UpdateUI("Right click to use Scythe", false);

        bool scytheUsed = false;
        while (!scytheUsed)
        {
            if (scythe.WasPressedThisFrame())
            {
                scytheUsed = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Spawn enemies and wait for them to be defeated
    private IEnumerator NightFightStage()
    {
        ui.UpdateUI("Defeat Enemies with the Scythe!", false);

        bool enemiesDefeated = false;
        for (int i = 0; i < EnemyCount; i++)
        {
            spawner.Spawn(spawner.enemyPrefab[1]);
        }
        while (!enemiesDefeated)
        {
            //Makes all items uncollectable during this stage
            CanCollectItems(false);

            if (EnemyCollection.childCount == 0)
            {
                StartCoroutine(worldClock.TimeChange());
                enemiesDefeated = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Wait for item pickup from zombies and water bucket at well
    private IEnumerator PickupItemsStage()
    {
        ui.UpdateUI("Pick up dropped items and bucket", false);

        bool itemsPickedUp = false;
        // Enable item pickup or spawn water bucket or something else
        CanCollectItems(true);

        while (!itemsPickedUp)
        {
            if (GroundItemCollection.childCount == 0)
            {
                itemsPickedUp = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Progress when seeds leave inventory
    private IEnumerator PlantCropsStage()
    {
        ui.UpdateUI("Plant the seeds in your inventory", false);

        bool cropsPlanted = false;
        player.GetComponent<InteractScript>().canPlant = true;
        while (!cropsPlanted)
        {
            if (inv.SearchForItem(seed) == 0)
            {
                cropsPlanted = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Progress when water bucket is filled or when well is interacted with
    private IEnumerator FillBucketStage()
    {
        ui.UpdateUI("Interact with the well to fill your bucket", false);

        bool wellInteraction = false;
        player.GetComponent<InteractScript>().canWater = true;
        while (!wellInteraction)
        {
            if(inv.IsWaterBucketEmpty() == false)
            {
                wellInteraction = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Progress when all crops are watered
    private IEnumerator WaterCropsStage()
    {
        ui.UpdateUI("Water your planted crops", false);

        bool cropsWatered = false;
        while (!cropsWatered)
        {
            if (soilManager.PlantsWatered() == 3)
            {
                cropsWatered = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Progress when all crops are killed/harvested
    private IEnumerator HarvestCropsStage()
    {
        ui.UpdateUI("Harvest your grown crops with your scythe", false);

        bool cropsHarvested = false;
        while (!cropsHarvested)
        {
            if (soilManager.TotalPlants() == 0)
            {
                cropsHarvested = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Progress when food is eaten from inventory (would like to rewrite later)
    private IEnumerator EatFoodStage()
    {
        ui.UpdateUI("Eat food to regain health", false);

        bool foodEaten = false;
        while (!foodEaten)
        {
            if (inv.SearchForItem(food) == 2)
            {
                player.GetComponent<PlayerHealth>().SetMaxHealth();
                foodEaten = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Progress when all produce is sold
    private IEnumerator SellItemsStage()
    {
        ui.UpdateUI("Go to the shop and press E to sell your harvested crops", false);

        bool itemsSold = false;
        shop.interactable = true;
        while (!itemsSold)
        {
            if (inv.SearchForItem(food) == 0)
            {
                itemsSold = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    private void CanCollectItems(bool flag)
    {
        player.GetComponentInChildren<ItemMagnet>().canPullItems = flag;
        foreach (Transform item in GroundItemCollection)
        {
            item.GetComponent<ItemDropScript>().canBeCollected = flag;
        }
    }

    private IEnumerator SetCheck()
    {
        ui.Checkmark(true);
        yield return new WaitForSeconds(2f);
    }
}
