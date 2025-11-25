using UnityEngine;
using System.Collections;

public class SpriteScript : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private Sprite downIdle;
    [SerializeField] private Sprite upIdle;
    [SerializeField] private Sprite leftIdle;
    [SerializeField] private Sprite rightIdle;

    Vector2 zeroVector = new Vector2(0, 0);


    //Rigidbody to detect movement
    private Rigidbody2D rb;

    enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    private Direction direction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //Assigning rigidbody to rb
        rb = GetComponent<Rigidbody2D>();

        //Assigning animator
        animator = GetComponent<Animator>();

        direction = Direction.DOWN;
    }

    // Update is called once per frame
    void Update()
    {   
        //reset animations
        animator.SetBool("isWalkingLeft", false);
        animator.SetBool("isWalkingRight", false);
        animator.SetBool("isWalkingUp", false);
        animator.SetBool("isWalkingDown", false);

        //If not moving
        if (rb.linearVelocity == zeroVector)
        {
            switch(direction)
            {
                case Direction.LEFT:
                    animator.SetBool("isIdleLeft", true);
                    break;
                case Direction.RIGHT:
                    animator.SetBool("isIdleRight", true);
                    break;
                case Direction.UP:
                    animator.SetBool("isIdleUp", true);
                    break;
                case Direction.DOWN:
                    animator.SetBool("isIdleDown", true);
                    break;
            }
        }
        else 
        {
        //reset animations
        animator.SetBool("isIdleLeft", false);
        animator.SetBool("isIdleRight", false);
        animator.SetBool("isIdleUp", false);
        animator.SetBool("isIdleDown", false); //add transition from down to idle 

        //Checking if the rigidbody is moving left or right and flipping the sprite accordingly
            if (rb.linearVelocity.x > 0.1f)
            {
                direction = Direction.RIGHT;
                animator.SetBool("isWalkingRight", true);
            }
            else if(rb.linearVelocity.x < -0.1f)
            {
                direction = Direction.LEFT;
                animator.SetBool("isWalkingLeft", true);
            }
            else if(rb.linearVelocity.y > 0.1f)
            {
                direction = Direction.UP;
                animator.SetBool("isWalkingUp", true);
            }
            else if(rb.linearVelocity.y < -0.1f)
            {
                direction = Direction.DOWN;
                animator.SetBool("isWalkingDown", true);
            }

        }
    }
}
