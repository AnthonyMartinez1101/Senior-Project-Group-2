using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrownWeb : MonoBehaviour
{

    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;
    public Sprite[] hitSprites; // Array of sprites for each hit level
    public SpriteRenderer sp;

    private bool hasHit = false; // prevent multiple hits
    private int hitCounter = 0;
    private InputAction scytheAction;

    void Start()
    {
        scytheAction = InputSystem.actions.FindAction("Scythe");
    }

    private void OnTriggerStay2D(Collider2D player)
    {
        if (hasHit) return; // already hit a player do nothing
        if (player.CompareTag("Player"))
        {
           hasHit = true;
           MovementScript movement = player.GetComponent<MovementScript>();
           if(movement != null)
           {
               StartCoroutine(ApplySlow(movement));
           } 
        }
    }

    private IEnumerator ApplySlow(MovementScript movement)
    {
        float originalSpeed = movement.speed;
        float originalDashSpeed = movement.dashSpeed;

        movement.speed = originalSpeed * slowMultiplier;
        movement.dashSpeed = originalDashSpeed * slowMultiplier;

        while (hitCounter < 3)
        {
            if (scytheAction.WasPressedThisFrame())
            {
                hitCounter++;

                if(hitCounter == 1)
                {
                    sp.sprite = hitSprites[0];
                }
                if(hitCounter == 2)
                {
                    sp.sprite = hitSprites[1];
                }
                if(hitCounter == 3)
                {
                    sp.sprite = hitSprites[2];
                }
            }
            yield return null;
        }

        if (movement != null) //if player is still alive
        {
            movement.speed = originalSpeed;
            movement.dashSpeed = originalDashSpeed;
        }
        Destroy(gameObject);
    }
}
