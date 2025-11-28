using UnityEngine;
using UnityEngine.InputSystem;

public class SpriteScript : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerInput playerInput;

    private Vector2 moveInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        rb.linearVelocity = moveInput * 5f; // your move speed
    }
    public void OnMove(InputAction.CallbackContext input)
    {   
        animator.SetBool("isWalking", true);
        if (input.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("lastX", moveInput.x);
            animator.SetFloat("lastY", moveInput.y);
        }
        moveInput = input.ReadValue<Vector2>();
        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
    }


}
