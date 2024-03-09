using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IGridCell<out TGridCell> where TGridCell : IGridCell<TGridCell>
{
  public Vector3Int GridPos { get; }
  public IEnumerable<TGridCell> GetNeighbours();
}

public class PathFinder<TGridCell> where TGridCell : IGridCell<TGridCell>
{
  private Func<TGridCell, bool> IsWalkableCell { get; }

  public PathFinder(Func<TGridCell, bool> walkableCellPredicate) =>
    IsWalkableCell = walkableCellPredicate;

  public IEnumerable<TGridCell> Find(TGridCell startCell, Func<TGridCell, bool> isTarget)
  {
    if (!FindTargetPoint(startCell, isTarget, out Point target)) yield break;

    foreach (Point point in target.GetPath())
      yield return point.Cell;
  }

  private bool FindTargetPoint(TGridCell startCell, Func<TGridCell, bool> isTargetCell, out Point targetPoint)
  {
    Queue<Point> pointsToCheck = new();
    HashSet<Vector3Int> ignoredGridPositions = new();

    pointsToCheck.Enqueue(new Point { Cell = startCell });
    ignoredGridPositions.Add(startCell.GridPos);
    while (pointsToCheck.Any())
    {
      Point currentPoint = pointsToCheck.Dequeue();
      if (isTargetCell(currentPoint.Cell))
      {
        targetPoint = currentPoint;
        return true;
      }

      foreach (TGridCell neighbour in currentPoint.Cell.GetNeighbours().Where(IsWalkableCell))
        if (ignoredGridPositions.Add(neighbour.GridPos))
          pointsToCheck.Enqueue(new Point { Cell = neighbour, PrevPoint = currentPoint });
    }

    targetPoint = null;
    return false;
  }

  public class Point
  {
    public TGridCell Cell;
    public Point PrevPoint;

    public IEnumerable<Point> GetPath()
    {
      Point point = this;
      while (point != null)
      {
        yield return point;
        point = point.PrevPoint;
      }
    }
  }
}