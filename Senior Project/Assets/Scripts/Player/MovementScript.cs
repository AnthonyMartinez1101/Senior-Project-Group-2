using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem; //New input system

public class MovementScript : MonoBehaviour
{
    //Speed and rigidbody of player
    public float speed = 5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    public float footStepTimer = 0.5f;
    private float currentFootStepTimer = 0f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 direction;
    private bool slowed = false;

    private Rigidbody2D rb;
    InputAction moveAction; // Detect WASD
    InputAction dashAction; // Detect dash button

    [SerializeField] private float speedBuffPercentage = 0f;
    [SerializeField] private float actualSpeed = 0f;
    [SerializeField] private float actualDashSpeed = 0f;

    private Attack attackScript;

    private Knockback knockback;

    [SerializeField] private ShopScript shop;

    private PlayerAudio playerAudio;

    [SerializeField] private GameObject walkDustParticles;

    //Called when the script is made
    private void Start()
    {
        //Assigns rb to the rigidbody component of player
        rb = GetComponent<Rigidbody2D>();

        //Assigns moveAction to detect movement input
        moveAction = InputSystem.actions.FindAction("Move");

        // Detects a dash input when spacebar is pressed
        dashAction = InputSystem.actions.FindAction("Dash");

        attackScript = GetComponent<Attack>();

        knockback = GetComponent<Knockback>();

        if(shop == null)
        {
            Debug.Log("MovementScript: Shop not assigned in inspector");
        }

        playerAudio = GetComponent<PlayerAudio>();
    }

    void Update()
    {
        if (dashAction.WasPressedThisFrame() && !CheckShop())
        {
            Dash();
        }
    }

    


    //Called at a fixed rate (not dependent on frame rate)
    private void FixedUpdate()
    {
        actualSpeed = speed * (1f + speedBuffPercentage / 100);
        actualDashSpeed = dashSpeed * (1f + speedBuffPercentage / 300);
        if (CheckShop()) 
        {
            rb.linearVelocity = Vector2.zero;
            return; 
        }

        if (isDashing)
        {
            rb.linearVelocity = direction * actualDashSpeed;
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
            }
        }
        else if (!knockback.IsKnockbackActive())
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            direction = moveValue.normalized;

            //Rotate the x rotation of dust walk paarticles to face opposite direction of movement
            if (walkDustParticles)
            {
                if (moveValue != Vector2.zero)
                {
                    float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
                    walkDustParticles.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }
            }


            if (!attackScript.IsMeleeing()) rb.linearVelocity = direction * actualSpeed;
            else rb.linearVelocity = direction * (actualSpeed / 2f);

            if(moveValue != Vector2.zero && !isDashing)
            {
                currentFootStepTimer -= Time.fixedDeltaTime;
                if(currentFootStepTimer <= 0f)
                {
                    playerAudio.PlayFootstep();
                    currentFootStepTimer = footStepTimer;
                }
            }
            else currentFootStepTimer = 0;

            if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.fixedDeltaTime;
        }
    }

    private void Dash()
    {
        if (!isDashing && dashCooldownTimer <= 0f && direction != Vector2.zero)
        {
            isDashing = true;
            dashTimer = dashDuration;
        }
    }

    private bool CheckShop()
    {
        return shop != null && shop.IsShopInUse();
    }

    public void SpeedBuff()
    {
        speedBuffPercentage += 0.5f;
        StatManager.Instance.AddSpeedBuff(0.5f);
    }

    public bool IsSlowed()
    {
        return slowed;
    }

    public void SetSlowed(bool flag)
    {
        slowed = flag;
    }
}
