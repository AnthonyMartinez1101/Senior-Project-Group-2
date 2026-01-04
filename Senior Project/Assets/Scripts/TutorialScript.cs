using System.Collections;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        yield return StartCoroutine(MovementStage());
        yield return StartCoroutine(NightFightStage());
        yield return StartCoroutine(PickupItemsStage());
        yield return StartCoroutine(PlantCropsStage());
        yield return StartCoroutine(FillBucketStage());
        yield return StartCoroutine(WaterCropsStage());
        yield return StartCoroutine(HarvestCropsStage());
        yield return StartCoroutine(EatFoodStage());
        yield return StartCoroutine(SellItemsStage());
        // Tutorial complete, swap scenes
    }

    // Wait for WASD and Space to be pressed
    private IEnumerator MovementStage()
    {
        yield return null;
    }

    private IEnumerator NightFightStage()
    {
        yield return null;
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
