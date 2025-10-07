using UnityEngine;
using UnityEngine.InputSystem;

public class AimScript : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public Transform aim;

    Vector2 playerDir;

    private bool useMouse = true;
    InputAction toggleInput;


    private void Start()
    {
        toggleInput = InputSystem.actions.FindAction("Toggle");
    }

    // Update is called once per frame
    void Update()
    {
        //PRESS T TO TOGGLE
        if(toggleInput.WasPressedThisFrame()) 
        {
            useMouse = !useMouse;
        }

        //If mouse mode is selected (Only apply if mouse is being used)
        if (useMouse && Mouse.current != null)
        {
            Vector3 screen = Mouse.current.position.ReadValue();
            Vector3 world = Camera.main.ScreenToWorldPoint(screen);
            world.z = aim.position.z;

            Vector2 dir = (world - aim.position).normalized;
            aim.up = -dir;
        }

        //Else, follow player movement
        else
        {
            //Get the player's movement direction from the Rigidbody2D's velocity
            playerDir = playerRB.linearVelocity;

            //Only update aim direction if player is moving
            if (playerDir.sqrMagnitude > 0.01f)
            {
                playerDir.Normalize();
                aim.up = -playerDir;
            }
        }
    }
}
