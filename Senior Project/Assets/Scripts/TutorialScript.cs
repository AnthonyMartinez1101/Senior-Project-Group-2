using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialScript : MonoBehaviour
{
    InputAction WASD;
    InputAction Space;

    public EnemySpawner spawner;
    public int EnemyCount = 3;
    public Transform EnemyCollection;
    public WorldClock worldClock;

    public Transform GroundItemCollection;

    public Inventory inv;
    public Item seed;

    private void Start()
    {
        WASD = InputSystem.actions.FindAction("Move");
        Space = InputSystem.actions.FindAction("Dash");

        // Lock mechanics here

        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        Debug.Log("Starting tutorial");
        yield return StartCoroutine(MovementStage());
        Debug.Log("Movement complete");
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
        Debug.Log("Tutorial complete, continuing to main game");
        // Tutorial complete, swap scenes
    }

    // Side note: Need to lock some mechanics such as shop (unlock when ready), and premature plant slicing
    // Prevent player from skipping ahead (lock planting, water filling, harvesting, eating, selling until tutorial reaches that stage)

    // Wait for WASD and Space to be pressed
    private IEnumerator MovementStage()
    {
        bool w = false, a = false, s = false, d = false, space = false;
        // Problem where each input needs to be pressed seperately (sorta)
        while (!(w && a && s && d && space))
        {
            if (WASD.WasPressedThisFrame())
            {
                Vector2 input = WASD.ReadValue<Vector2>();
                if (input.y > 0) w = true;
                if (input.y < 0) s = true;
                if (input.x < 0) a = true;
                if (input.x > 0) d = true;
            }
            if (Space.WasPressedThisFrame())
            {
                space = true;
            }
            yield return null;
        }
    }

    // Spawn enemies and wait for them to be defeated
    private IEnumerator NightFightStage()
    {
        bool enemiesDefeated = false;
        for (int i = 0; i < EnemyCount; i++)
        {
            spawner.Spawn(spawner.enemyPrefab[1]);
        }
        while (!enemiesDefeated)
        {
            if (EnemyCollection.childCount == 0)
            {
                StartCoroutine(worldClock.TimeChange());
                enemiesDefeated = true;
            }
            yield return null;
        }
    }

    // Wait for item pickup from zombies and water bucket at well
    private IEnumerator PickupItemsStage()
    {
        bool itemsPickedUp = false;
        // Enable item pickup or spawn water bucket or something else
        while (!itemsPickedUp)
        {
            if (GroundItemCollection.childCount == 0)
            {
                itemsPickedUp = true;
            }
            yield return null;
        }
    }

    // Progress when seeds leave inventory
    private IEnumerator PlantCropsStage()
    {
        bool cropsPlanted = false;
        while (!cropsPlanted)
        {
            if (inv.SearchForItem(seed) == false)
            {
                cropsPlanted = true;
            }
            yield return null;
        }
    }

    // Progress when water bucket is filled or when well is interacted with
    private IEnumerator FillBucketStage()
    {
        bool wellInteraction = false;
        while (!wellInteraction)
        {

            yield return null;
        }
    }

    private IEnumerator WaterCropsStage()
    {
        yield return null;
    }

    private IEnumerator HarvestCropsStage()
    {
        yield return null;
    }

    private IEnumerator EatFoodStage()
    {
        yield return null;
    }

    private IEnumerator SellItemsStage()
    {
        yield return null;
    }
}
