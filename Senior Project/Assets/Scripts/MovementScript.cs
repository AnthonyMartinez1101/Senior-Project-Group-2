using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{

    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;


    //private void Start()
    //{
    //    //Assign the Rigidbody2D component to rb
    //    rb.GetComponent<Rigidbody2D>();
    //}

    //void FixedUpdate()
    //{
    //    Vector2 v = Vector2.ClampMagnitude(moveInput, 1f) * moveSpeed;
    //    rb.linearVelocity = v;
    //}

    //// Hook this to Player Input -> Move (Unity Events)
    //public void Move(InputAction.CallbackContext ctx)
    //{
    //    moveInput = ctx.ReadValue<Vector2>();
    //}
}
