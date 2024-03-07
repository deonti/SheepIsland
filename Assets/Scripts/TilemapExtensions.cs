using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions
{
  public static TileBase GetTile(this Tilemap tilemap, Vector2 position) => 
    tilemap.GetTile(tilemap.WorldToCell(position));
}