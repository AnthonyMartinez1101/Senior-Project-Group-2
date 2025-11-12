using UnityEngine;
using UnityEngine.InputSystem; //New input system

public class MovementScript : MonoBehaviour
{
    //Speed and rigidbody of player
    public float speed = 5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 direction;

    private Rigidbody2D rb;
    InputAction moveAction; // Detect WASD
    InputAction dashAction; // Detect dash button

    private Attack attackScript;

    private Knockback knockback;

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
    }

    void Update()
    {
        if (dashAction.WasPressedThisFrame())
        {
            Dash();
        }
    }


    //Called at a fixed rate (not dependent on frame rate)
    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = direction * dashSpeed;
            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashCooldownTimer = dashCooldown;
            }
        }
        else if(!knockback.IsKnockbackActive())
        {
            Vector2 moveValue = moveAction.ReadValue<Vector2>();
            direction = moveValue.normalized;
            if(!attackScript.IsMeleeing()) rb.linearVelocity = direction * speed;
            else rb.linearVelocity = direction * (speed / 2f);


            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.fixedDeltaTime;
            }
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
}
