using UnityEngine;
using UnityEngine.InputSystem; //New input system

public class MovementScript : MonoBehaviour
{
    //Speed and rigidbody of player
    public float speed = 5f;
    private Rigidbody2D rb;

    //Creating an action to detect movement input
    InputAction moveAction;

    //Called when the script is made
    private void Start()
    {
        //Assigns rb to the rigidbody component of player
        rb = GetComponent<Rigidbody2D>();

        //Assigns moveAction to detect movement input
        moveAction = InputSystem.actions.FindAction("Move");
    }


    //Called at a fixed rate (not dependent on frame rate)
    private void FixedUpdate()
    {
        //Saves the value of movement input as a vector
        Vector2 moveValue = moveAction.ReadValue<Vector2>();

        //Applies movement to the rigidbody
        rb.linearVelocity = new Vector2(moveValue.x * speed, moveValue.y * speed);
    }   
}
