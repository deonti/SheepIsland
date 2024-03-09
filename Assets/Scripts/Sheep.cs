using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using Random = System.Random;

public class Sheep : MonoBehaviour
{
  [SerializeField] private float _movementSpeedMin = 0.5f;
  [SerializeField] private float _movementSpeedMax = 3f;

  private static readonly Collider2D[] _overlaps = new Collider2D[10];
  private static readonly Random _random = new();

  private Ground _ground;
  private float _movementSpeed;
  private readonly Stack<Vector3> _movementPath = new();
  private PathFinder<Ground.Cell> _pathFinder;

  private void OnEnable()
  {
    _ground = _ground ? _ground : FindObjectOfType<Ground>();
    if (!_ground)
      Destroy(gameObject);

    _pathFinder = new PathFinder<Ground.Cell>(IsWalkableForThisSheep);
    _movementSpeed = Mathf.Lerp(_movementSpeedMin, _movementSpeedMax, (float)_random.NextDouble());
  }

  private void Update()
  {
    UpdatePath();
    ProcessMovement();
    ProcessEating();
  }

  private void UpdatePath()
  {
    _movementPath.Clear();
    Ground.Cell startCell = _ground.GetCell(transform.position);
    foreach (Ground.Cell cell in _pathFinder.Find(startCell, CellHasAccessibleGrass))
      _movementPath.Push(cell.WorldPos);

    if (_movementPath.Count > 1)
      _movementPath.Pop();
  }

  private bool IsWalkableForThisSheep(Ground.Cell cell) =>
    cell.IsWalkable && !cell.HasAnotherSheep(this);

  private bool CellHasAccessibleGrass(Ground.Cell cell) =>
    IsWalkableForThisSheep(cell) && cell.HasAnyGrass();

  private void ProcessMovement()
  {
    if (!_movementPath.Any()) return;

    Vector3 nextPathPoint = _movementPath.Peek();
    UpdateDirection(nextPathPoint);

    transform.position = Vector3.MoveTowards(transform.position, nextPathPoint, _movementSpeed * Time.deltaTime);
    if (Vector3.Distance(transform.position, nextPathPoint) < 0.01f)
      _movementPath.Pop();
  }

  private void UpdateDirection(Vector3 targetPoint)
  {
    Vector3 scale = transform.localScale;
    scale.x = (targetPoint - transform.position).x switch
    {
      <= -0.1f => -1,
      >= 0.1f => 1,
      _ => scale.x
    };
    transform.localScale = scale;
  }

  private void ProcessEating()
  {
    if (_movementPath.Any()) return;

    int overlapsCount = Physics2D.OverlapPointNonAlloc(transform.position, _overlaps);

    for (int i = 0; i < overlapsCount; i++)
    {
      if (!_overlaps[i].TryGetComponent(out Grass grass)) continue;

      Destroy(grass.gameObject);
      break;
    }
  }
}

public static partial class CellExtensions
{
  private static readonly Collider2D[] _sheepOverlaps = new Collider2D[42];

  public static bool HasAnotherSheep(this Ground.Cell cell, Sheep sheep)
  {
    int overlapsCount = Physics2D.OverlapPointNonAlloc(cell.WorldPos, _sheepOverlaps);

    for (int i = 0; i < overlapsCount; i++)
      if (_sheepOverlaps[i].TryGetComponent(out Sheep someSheep) && someSheep != sheep)
        return true;
    return false;
  }

  public static bool HasAnySheep(this Ground.Cell cell)
  {
    int overlapsCount = Physics2D.OverlapPointNonAlloc(cell.WorldPos, _sheepOverlaps);

    for (int i = 0; i < overlapsCount; i++)
      if (_sheepOverlaps[i].HasComponent<Sheep>())
        return true;
    return false;
  }
}