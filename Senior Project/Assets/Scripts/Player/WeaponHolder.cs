using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class WeaponHolder : MonoBehaviour
{
    //The 3 children of weapon holder
    private GameObject[] weaponHolders;

    Vector2 playerDir;

    private bool canAim = true;
    private bool controllerConnected = false;

    InputAction toggleInput;
    InputAction rightStick;

    private float stickDeadzone = 0.1f;
    private Vector2 stickPos = Vector2.zero;

    public Rigidbody2D playerRB; // Reference to the player's Rigidbody2D

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get the 3 children of weapon holder
        weaponHolders = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            weaponHolders[i] = transform.GetChild(i).gameObject;
        }
        toggleInput = InputSystem.actions.FindAction("Toggle");
        rightStick = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        //PRESS T TO TOGGLE
        if (toggleInput.WasPressedThisFrame())
        {
            canAim = !canAim;
        }
        stickPos = rightStick.ReadValue<Vector2>();

        // Scan for type of input and update controllerConnected
        ScanInputType();

        //if (controllerConnected)
        //{
        //    Cursor.visible = false;
        //    Cursor.lockState = CursorLockMode.Locked;
        //}
        //else
        //{
        //    Cursor.visible = true;
        //    Cursor.lockState = CursorLockMode.None;
        //}

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
    }


    public void SetWeaponHolder(int weaponIndex)
    {
        //Set the active state of the weapon holders based on the selected weaponIndex
        for (int i = 0; i < weaponHolders.Length; i++)
        {
            weaponHolders[i].SetActive(i == weaponIndex);
        }
    }


    public Transform GetHolderTransform()
    {
        //Return the transform of the active weapon holder
        for (int i = 0; i < weaponHolders.Length; i++)
        {
            if (weaponHolders[i].activeSelf)
            {
                var holderData = weaponHolders[i].GetComponent<GunHolderData>();
                return holderData.muzzlePosition;
            }
        }
        return transform; // Return the transform of the weapon holder parent itself if no active weapon holder is found
    }


    private void UpdateAimOnController()
    {
        if (Mathf.Abs(stickPos.x) > stickDeadzone || Mathf.Abs(stickPos.y) > stickDeadzone)
        {
            Vector3 holderPos = transform.position;
            Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(holderPos);
            Vector3 stickOffset = new Vector3(stickPos.x, stickPos.y, 0) * 100f;
            Vector3 targetScreenPos = playerScreenPos + stickOffset;

            Vector3 world = Camera.main.ScreenToWorldPoint(targetScreenPos);    
            world.z = holderPos.z;

            Vector2 dir = (world - holderPos).normalized;
            transform.up = -dir;

            // Update weapon holding (flip based on aim direction)
            bool isLeft = world.x < transform.parent.position.x;
            Vector3 localPos = transform.localPosition;
            localPos.x = isLeft ? -Mathf.Abs(localPos.x) : Mathf.Abs(localPos.x);
            transform.localPosition = localPos;

            Vector3 localScale = transform.localScale;
            localScale.x = isLeft ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
            transform.localScale = localScale;
        }
    }


    private void UpdateAimOnMouse()
    {
        Vector3 holderPos = transform.position;
        Vector3 mousePos = Mouse.current.position.ReadValue();

        if (float.IsNaN(mousePos.x) || float.IsNaN(mousePos.y) || float.IsInfinity(mousePos.x) || float.IsInfinity(mousePos.y))
        {
            return; // Skip if mouse position is invalid
        }

        Vector3 world = Camera.main.ScreenToWorldPoint(mousePos);
        world.z = holderPos.z;

        Vector2 dir = (world - holderPos).normalized;
        transform.up = -dir;

        // Update weapon holding (flip based on aim direction)
        bool isLeft = world.x < transform.parent.position.x;
        Vector3 localPos = transform.localPosition;
        localPos.x = isLeft ? -Mathf.Abs(localPos.x) : Mathf.Abs(localPos.x);
        transform.localPosition = localPos;

        Vector3 localScale = transform.localScale;
        localScale.x = isLeft ? -Mathf.Abs(localScale.x) : Mathf.Abs(localScale.x);
        transform.localScale = localScale;
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
