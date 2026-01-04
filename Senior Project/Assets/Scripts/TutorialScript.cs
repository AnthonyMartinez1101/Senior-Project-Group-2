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

    private void Start()
    {
        WASD = InputSystem.actions.FindAction("Move");
        Space = InputSystem.actions.FindAction("Dash");

        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
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

    // Wait for WASD and Space to be pressed
    private IEnumerator MovementStage()
    {
        bool w = false, a = false, s = false, d = false, space = false;
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
            spawner.Spawn();
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

    private IEnumerator PickupItemsStage()
    {
        yield return null;
    }

    private IEnumerator PlantCropsStage()
    {
        yield return null;
    }

    private IEnumerator FillBucketStage()
    {
        yield return null;
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
