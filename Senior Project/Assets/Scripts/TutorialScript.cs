using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

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

    public GameObject tutoritalEnemy;

    public GameObject dayEnemies;

    InputAction toggleInput;
    InputAction rightStick;
    private bool controllerConnected = false;

    private void Start()
    {
        WASD = InputSystem.actions.FindAction("Move");
        Space = InputSystem.actions.FindAction("Dash");

        scythe = InputSystem.actions.FindAction("Scythe");

        toggleInput = InputSystem.actions.FindAction("ToggleInput");
        rightStick = InputSystem.actions.FindAction("Look");

        // Lock mechanics here
        player.GetComponent<InteractScript>().canPlant = false;
        player.GetComponent<InteractScript>().canWater = false;
        player.GetComponent<InteractScript>().canEat = false;
        player.GetComponent<PlayerHealth>().isInvincible = true;
        //player.GetComponent<PlayerHealth>().SetHealth(15f);
        CanCollectItems(false);
        shop.interactable = false;

        ui.UpdateUI("Use WASD or left stick to move around", false);

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

        yield return StartCoroutine(DefeatDayEnemies());
        Debug.Log("Defeat day enemies complete");

        yield return StartCoroutine(HarvestCropsStage());
        Debug.Log("Harvest crops complete");

        yield return StartCoroutine(CollectProduceStage());
        Debug.Log("Collect produce complete");

        yield return StartCoroutine(EatFoodStage());
        Debug.Log("Eat food complete");

        yield return StartCoroutine(SellItemsStage());
        Debug.Log("Sell items complete");

        yield return StartCoroutine(BuyItemsStage());
        Debug.Log("Buy items complete");

        ui.UpdateUI("Tutorial Complete!", true);
        Debug.Log("Tutorial complete, continuing to main game");

        yield return new WaitForSeconds(3f);
        // Tutorial complete, swap scenes (use fade to black?)
        SceneManager.LoadSceneAsync(0);
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
        if (controllerConnected) ui.UpdateUI("Use right face button to dash while moving", false);
        else ui.UpdateUI("Use Space to dash while moving", false);

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
        if (controllerConnected) ui.UpdateUI("Press left trigger to use Scythe", false);
        else ui.UpdateUI("Right click to use Scythe", false);

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
            spawner.Spawn(tutoritalEnemy);
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
        if (controllerConnected) ui.UpdateUI("Press right trigger or bottom face button on a highlighted soil plot to plant seeds", false);
        else ui.UpdateUI("Plant the seeds by interacting with a highlighted soil plot", false);

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
        if (controllerConnected) ui.UpdateUI("Press right trigger or bottom face button near the well to fill your bucket", false);
        else ui.UpdateUI("Interact with the well to fill your bucket", false);

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
        if (controllerConnected) ui.UpdateUI("Using your bucket, press right trigger or bottom face button near your crops to water them", false);
        else ui.UpdateUI("Use your bucket to interact and water the planted crops", false);

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

    private IEnumerator DefeatDayEnemies()
    {
        ui.UpdateUI("Bugs are eating your crops! Defeat them with your scythe!", false);
        bool enemiesDefeated = false;
        dayEnemies.SetActive(true);
        while (!enemiesDefeated)
        {
            if (dayEnemies.transform.childCount == 0)
            {
                enemiesDefeated = true;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Progress when all crops are killed/harvested
    private IEnumerator HarvestCropsStage()
    {
        ui.UpdateUI("Harvest your fully grown crops with your scythe by attacking them", false);

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

    // Progress when all produce is picked up
    private IEnumerator CollectProduceStage()
    {
        ui.UpdateUI("Pick up dropped produce", false);

        bool itemsPickedUp = false;

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


    // Progress when food is eaten from inventory (would like to rewrite later)
    private IEnumerator EatFoodStage()
    {
        ui.UpdateUI("Interact and eat the harvested produce to regain health", false);

        player.GetComponent<InteractScript>().canEat = true;

        bool foodEaten = false;
        while (!foodEaten)
        {
            if (inv.SearchForItem(food) == 2)
            {
                player.GetComponent<PlayerHealth>().SetMaxHealth();
                foodEaten = true;
                player.GetComponent<InteractScript>().canEat = false;
            }
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }

    // Progress when all produce is sold
    private IEnumerator SellItemsStage()
    {
        if (controllerConnected) ui.UpdateUI("Press top face button near the shop to open it and sell your products", false);
        else ui.UpdateUI("Go to the shop and open it with E, then sell your products", false);

        shop.interactable = true;
        while (!inv.HasSold)
        {
            yield return null;
        }
        yield return StartCoroutine(SetCheck());
    }


    private IEnumerator BuyItemsStage()
    {
        ui.UpdateUI("Buy some items you can afford, and open your package with your trusty scythe!", false);

        bool hasOpenedPackage = false;
        bool foundPackage = false;
        Package package = Object.FindFirstObjectByType<Package>();
        while (!hasOpenedPackage)
        {
            if (!package && !foundPackage)
            {
                package = Object.FindFirstObjectByType<Package>();
            }
            if (package)
            {
                foundPackage = true;
            }
            if(foundPackage && !package)
            {
                hasOpenedPackage = true;
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
        // run scaninputtype for a few seconds to allow player to switch input types if they want
        float timer = 2f;
        while (timer > 0f)
        {
            ScanInputType();
            timer -= Time.deltaTime;
            yield return null;
        }
    }
    
    
    private void ScanInputType()
    {
        // Keyboard & mouse input
        if (controllerConnected)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                controllerConnected = false;
            }
            if (Mouse.current != null && (
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame ||
                Mouse.current.delta.ReadValue() != Vector2.zero))
            {
                controllerConnected = false;
            }
        }

        // Gamepad input
        else
        {
            if (Gamepad.current != null)
            {
                foreach (var control in Gamepad.current.allControls)
                {
                    if (control is ButtonControl btn && btn.wasPressedThisFrame)
                    {
                        controllerConnected = true;
                    }
                }
                if (Gamepad.current.leftStick.ReadValue() != Vector2.zero ||
                    Gamepad.current.rightStick.ReadValue() != Vector2.zero)
                {
                    controllerConnected = true;
                }
            }
        }
    }
}