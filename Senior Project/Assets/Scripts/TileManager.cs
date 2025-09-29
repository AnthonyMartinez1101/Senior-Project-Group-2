using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap; // Tilemap to manage

    [SerializeField] private Tile hiddenTile; // Tile to set

    void Start()
    {
        foreach (var position in interactableMap.cellBounds.allPositionsWithin) // Iterate through all positions in the tilemap's bounds
        {
            TileBase tile = interactableMap.GetTile(position);

            if (tile != null && tile.name == "dirt_interactable_visible")
            {
                interactableMap.SetTile(position, hiddenTile); // Set the tile at the current position to the hidden tile
            }
        }
    }

    public bool IsInteractable(Vector3Int position)
    {
        TileBase tile = interactableMap.GetTile(interactableMap.WorldToCell(position)); // Gets tile at world position

        if (tile != null)
        {
            if (tile.name == "dirt_interactable")
            {
                return true;
            }
        }

        return false;
    }
}
