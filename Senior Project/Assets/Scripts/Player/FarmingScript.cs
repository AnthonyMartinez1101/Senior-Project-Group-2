using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class FarmingScript : MonoBehaviour
{
    InputAction interactButton;

    public GameObject seed;
    public Transform plantParentObject;

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
                Vector3 spawnPos = GameManager.instance.tileManager.interactableMap.GetCellCenterWorld(playerPosition);
                plantSeed(spawnPos);
            }
            else
            {
                // Play sound maybe?
            }
        }
    }

    void plantSeed(Vector3 pos)
    {
        Instantiate(seed, pos, Quaternion.identity, plantParentObject);
    }
}
