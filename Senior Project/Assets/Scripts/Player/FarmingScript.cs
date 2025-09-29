using UnityEngine;
using UnityEngine.InputSystem;

public class FarmingScript : MonoBehaviour
{
    InputAction interactButton;


    private void Start()
    {
        interactButton = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        if (interactButton.WasPressedThisFrame())
        {
            Vector3Int playerPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);

            if (GameManager.instance.tileManager.IsInteractable(playerPosition))
            {
                Debug.Log("Tile is interactable");
            }
            else
            {
                Debug.Log("Tile is not interactable");
            }
        }
    }
}
