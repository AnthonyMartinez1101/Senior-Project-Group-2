using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class AimScript : MonoBehaviour
{
    public Rigidbody2D playerRB;
    public Transform aim;

    Vector2 playerDir;

    private bool canAim = true;
    private bool controllerConnected = false;

    InputAction toggleInput;
    InputAction rightStick;

    private float stickDeadzone = 0.1f;
    private Vector2 stickPos = Vector2.zero;

    private void Start()
    {
        toggleInput = InputSystem.actions.FindAction("Toggle");
        rightStick = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        //PRESS T TO TOGGLE
        if(toggleInput.WasPressedThisFrame()) 
        {
            canAim = !canAim;
        }
        stickPos = rightStick.ReadValue<Vector2>(); 

        // Scan for type of input and update controllerConnected
        ScanInputType();

        if (controllerConnected)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        //If twinstick controls are enabled
        if (canAim && Mouse.current != null && Camera.main != null)
        {
            if (controllerConnected)
            {
                UpdateAimOnController();
            }
            else
            {
                UpdateAimOnMouse();
            }
        }

        //Else, follow player movement
        else
        {
            UpdateAimOnMovement();
        }
    }


    private void UpdateAimOnController()
    {
        if (Mathf.Abs(stickPos.x) > stickDeadzone || Mathf.Abs(stickPos.y) > stickDeadzone)
        {
            Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(aim.position);
            Vector3 stickOffset = new Vector3(stickPos.x, stickPos.y, 0) * 100f;
            Vector3 targetScreenPos = playerScreenPos + stickOffset;

            Vector3 world = Camera.main.ScreenToWorldPoint(targetScreenPos);
            world.z = aim.position.z;

            Vector2 dir = (world - aim.position).normalized;
            aim.up = -dir;
        }
    }


    private void UpdateAimOnMouse()
    {
        //Gets mouse position on the scree and converts it to world position
        Vector3 mousePos = Mouse.current.position.ReadValue();

        if (float.IsNaN(mousePos.x) || float.IsNaN(mousePos.y) || float.IsInfinity(mousePos.x) || float.IsInfinity(mousePos.y))
        {
            return; // Skip if mouse position is invalid
        }

        Vector3 world = Camera.main.ScreenToWorldPoint(mousePos);
        world.z = aim.position.z;

        //Sets the aim's up direction to point towards the mouse position
        Vector2 dir = (world - aim.position).normalized;
        aim.up = -dir;
    }


    private void UpdateAimOnMovement()
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


    private void ScanInputType()
    {
        // Keyboard & mouse input
        if (controllerConnected)
        {
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                controllerConnected = false;
            }
            if (Mouse.current != null && (
                Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame ||
                Mouse.current.delta.ReadValue() != Vector2.zero))
            {
                controllerConnected = false;
            }
        }

        // Gamepad input
        else
        {
            if (Gamepad.current != null)
            {
                foreach (var control in Gamepad.current.allControls)
                {
                    if (control is ButtonControl btn && btn.wasPressedThisFrame)
                    {
                        controllerConnected = true;
                    }
                }
                if (Gamepad.current.leftStick.ReadValue() != Vector2.zero ||
                    Gamepad.current.rightStick.ReadValue() != Vector2.zero)
                {
                    controllerConnected = true;
                }
            }
        }
    }
}
