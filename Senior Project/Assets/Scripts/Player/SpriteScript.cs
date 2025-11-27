using UnityEngine;
using UnityEngine.InputSystem;

public class SpriteScript : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerInput playerInput;

    private Vector2 moveInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
    }

    private void OnMove(InputAction.CallbackContext input)
    {
        moveInput = input.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * 5f; // your move speed

        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
   
    }
}
