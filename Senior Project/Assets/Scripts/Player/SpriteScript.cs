using UnityEngine;

public class SpriteScript : MonoBehaviour
{
    //Players sprite renderer duh
    private SpriteRenderer playerSprite;

    //Rigidbody to detect movement
    private Rigidbody2D rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Assigning sprite renderer to playerSprite
        playerSprite = GetComponent<SpriteRenderer>();

        //Assigning rigidbody to rb
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Checking if the rigidbody is moving left or right and flipping the sprite accordingly
        if (rb.linearVelocity.x > 0.15f)
        {
            playerSprite.flipX = false;
        }
        else if(rb.linearVelocity.x < 0.15f)
        {
            playerSprite.flipX = true;
        }
    }
}
