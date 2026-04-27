using UnityEngine;
using UnityEngine.InputSystem;

public class SkipDay : MonoBehaviour
{
    public WorldClock worldClock;

    private bool inRangeToSkip = false;

    InputAction openShopAction;

    public SpriteRenderer image;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!worldClock)
        {
            Debug.LogError("WorldClock reference not set in SkipDay script.");
        }
        openShopAction = InputSystem.actions.FindAction("OpenShop");
    }

    // Update is called once per frame
    void Update()
    {
        if (!worldClock) return;

        if (inRangeToSkip && openShopAction.triggered)
        {
            worldClock.SkipDay();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && worldClock.IsDay())
        {
            inRangeToSkip = true;
            image.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRangeToSkip = false;
            image.enabled = false;
        }
    }
}
