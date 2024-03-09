using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ground : MonoBehaviour
{
  [SerializeField] private Tilemap _tilemap;
  [SerializeField] private TileBase _walkableTile;

  public Cell GetCell(Vector3 worldPos) =>
    GetCell(_tilemap.WorldToCell(worldPos));

  public IEnumerable<Cell> GetCells()
  {
    foreach (Vector3Int gridPos in _tilemap.cellBounds.allPositionsWithin)
      yield return GetCell(gridPos);
  }

  public IEnumerable<Cell> GetCells(Func<Cell, bool> predicate) =>
    GetCells().Where(predicate);

  private Cell GetCell(Vector3Int gridPos) =>
    new(this, gridPos);

  public readonly struct Cell : IGridCell<Cell>
  {
    public Vector3Int GridPos { get; }
    public Vector3 WorldPos => _ground._tilemap.layoutGrid.GetCellCenterWorld(GridPos);
    public bool IsWalkable => _ground._tilemap.GetTile(GridPos) == _ground._walkableTile;
    public bool IsValid => _ground;

    private static readonly Vector3Int[] _neighbourGridPosOffsets =
    {
      new(+0, +1), // up
      new(+1, +0), // right
      new(+0, -1), // down
      new(-1, +0), // left
    };

    private readonly Ground _ground;

    public Cell(Ground ground, Vector3Int gridPos) : this()
    {
      _ground = ground;
      GridPos = gridPos;
    }

    public IEnumerable<Cell> GetNeighbours()
    {
      foreach (Vector3Int offset in _neighbourGridPosOffsets)
        yield return _ground.GetCell(GridPos + offset);
    }
  }
}