using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundInfo : MonoBehaviour
{
  [SerializeField] private Tilemap _tilemap;
  [SerializeField] private TileBase _walkableTile;

  private readonly Collider2D[] _overlaps = new Collider2D[10];

  public BoundsInt.PositionEnumerator AllCells => _tilemap.cellBounds.allPositionsWithin;

  public Vector3Int WorldToCell(Vector3 position) => 
    _tilemap.WorldToCell(position);

  public Vector3 CellToWorld(Vector3Int cell) => 
    _tilemap.CellToWorld(cell);

  public CellInfo GetCellInfo(Vector3Int cell)
  {
    CellInfo info = new(cell, CellToWorld(cell), _tilemap.GetTile(cell) == _walkableTile);
    if (!info.IsWalkable)
      return info;

    // int overlapsCount = Physics2D.OverlapPointNonAlloc(CellToWorld(cell), _overlaps);
    //
    // for (int i = 0; i < overlapsCount; i++)
    // {
    //   info.HasGrass = info.HasGrass || _overlaps[i].HasComponent<Grass>();
    //   info.HasSheep = info.HasSheep || _overlaps[i].HasComponent<Sheep>();
    // }

    return info;
  }

  // private readonly Random _random = new();
  //
  // public bool TryGetRandomUsedCellPosition(Func<CellState, bool> condition, out Vector3 position)
  // {
  //   BoundsInt bounds = _tilemap.cellBounds;
  //   List<Vector2> cells = new();
  //   foreach (Vector3Int cell in bounds.allPositionsWithin)
  //   {
  //     Vector3 cellPosition = _tilemap.CellToWorld(cell);
  //     if (condition(GetCellState(cellPosition)))
  //       cells.Add(cellPosition);
  //   }
  //
  //   if (cells.Count == 0)
  //   {
  //     position = default;
  //     return false;
  //   }
  //
  //   position = cells[_random.Next(cells.Count)];
  //   return true;
  // }

  public struct CellInfo
  {
    public readonly Vector3Int Cell;
    public readonly Vector3 WorldPos;
    public readonly bool IsWalkable;
    // public bool HasSheep;
    // public bool HasGrass;

    public CellInfo(Vector3Int cell, Vector3 worldPos, bool isWalkable) : this()
    {
      IsWalkable = isWalkable;
      WorldPos = worldPos;
      Cell = cell;
    }
  }
}