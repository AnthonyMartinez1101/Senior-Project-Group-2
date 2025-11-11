using UnityEngine;
using System.Collections;

public class SpriteScript : MonoBehaviour
{
    //Players sprite renderer duh
    private SpriteRenderer sprite;

    //Rigidbody to detect movement
    private Rigidbody2D rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Assigning sprite renderer to playerSprite
        sprite = GetComponent<SpriteRenderer>();

        //Assigning rigidbody to rb
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checking if the rigidbody is moving left or right and flipping the sprite accordingly
        if (rb.linearVelocity.x > 0.1f)
        {
            sprite.flipX = false;
        }
        else if(rb.linearVelocity.x < -0.1f)
        {
            sprite.flipX = true;
        }
    }
}
