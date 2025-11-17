using UnityEngine;
using System.Collections;

public class SpriteScript : MonoBehaviour
{
    //Players sprite renderer duh
    private SpriteRenderer sr;

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
        //Assigning sprite renderer to playerSprite
        sr = GetComponent<SpriteRenderer>();

        //Assigning rigidbody to rb
        rb = GetComponent<Rigidbody2D>();

        direction = Direction.DOWN;
    }

    // Update is called once per frame
    void Update()
    {
        //If not moving
        if(rb.linearVelocity == zeroVector)
        {
            switch(direction)
            {
                case Direction.LEFT:
                    sr.sprite = leftIdle;
                    break;
                case Direction.RIGHT:
                    sr.sprite = rightIdle;
                    break;
                case Direction.UP:
                    sr.sprite = upIdle;
                    break;
                case Direction.DOWN:
                    sr.sprite = downIdle;
                    break;
            }
        }



        //Checking if the rigidbody is moving left or right and flipping the sprite accordingly
        if (rb.linearVelocity.x > 0.1f)
        {
            //Set enum to right
            //playing right walk animation
        }
        else if(rb.linearVelocity.x < -0.1f)
        {
            //enum to left
        }
        //checking for y
    }
}
