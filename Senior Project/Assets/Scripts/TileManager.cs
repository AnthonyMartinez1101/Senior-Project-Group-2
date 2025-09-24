using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap; // Tilemap to manage
    [SerializeField] private Tile hiddenTile; // Tile to set

    void Start()
    {
        foreach(var position in interactableMap.cellBounds.allPositionsWithin) // Iterate through all positions in the tilemap's bounds
        {
            interactableMap.SetTile(position, hiddenTile); // Set the tile at the current position to the hidden tile
        }
    }
}
