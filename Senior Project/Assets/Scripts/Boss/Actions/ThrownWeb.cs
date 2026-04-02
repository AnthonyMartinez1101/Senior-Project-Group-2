using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrownWeb : MonoBehaviour
{
    public float lifetime = 60f;
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;
    public Sprite[] hitSprites; // Array of sprites for each hit level
    public SpriteRenderer sp;

    private bool hasHit = false; // prevent multiple hits
    private int hitCounter = 0;
    private InputAction scytheAction;
    private GameObject Player;
    void Start()
    {
        scytheAction = InputSystem.actions.FindAction("Scythe");
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime < 0 && !hasHit)
        {
            Destroy(gameObject);
        }
        if(hasHit && Player)
        {
            transform.position = Player.transform.position;
        }
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (hasHit) return; // already hit a player do nothing
        if (collider.CompareTag("Player"))
        {
           hasHit = true;
           Player = collider.gameObject;
           MovementScript movement = collider.GetComponent<MovementScript>();
           if(movement != null)
           {
                if(!movement.IsSlowed())
                {
                    StartCoroutine(ApplySlow(movement));
                }
                else
                    Destroy(gameObject);
           } 
        }
    }

    private IEnumerator ApplySlow(MovementScript movement)
    {
        movement.SetSlowed(true);
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
            movement.SetSlowed(false);
        }
        Destroy(gameObject);
    }
}
