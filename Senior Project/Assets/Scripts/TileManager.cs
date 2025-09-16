using UnityEngine;
using UnityEngine.Tilemaps;
public class TileManager
{
   [SerializeField] private Tilemap interactableTile; // Tilemap to manage
   [SerializeField] private Tile hiddenTile; // Tile to set

   void Start()
   {
      foreach(var position in interactableTile.cellBounds.allPositionsWithin) // Iterate through all positions in the tilemap's bounds
      {
        interactableTile.SetTile(position, hiddenTile); // Set the tile at the current position to the hidden tile
      }
   }
}
