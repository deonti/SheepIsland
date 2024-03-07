using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Sheep : MonoBehaviour
{
  [SerializeField] private float _movementSpeedMin = 0.5f;
  [SerializeField] private float _movementSpeedMax = 3f;

  private GroundInfo _ground;

  private readonly Stack<Vector3> _movementPath = new();

  static Random _random = new Random();
  private float _movementSpeed;
  
  private void OnEnable()
  {
    _ground = _ground ? _ground : FindObjectOfType<GroundInfo>();
    if (!_ground)
      Destroy(gameObject);

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

    Vector3Int startCell = _ground.WorldToCell(transform.position);
    PathCellData endData = null;

    Queue<PathCellData> openCellsData = new();
    openCellsData.Enqueue(new PathCellData { Cell = startCell });
    List<Vector3Int> closedCells = new() { startCell };

    while (openCellsData.Any())
    {
      PathCellData data = openCellsData.Dequeue();
      Vector3 cellWorldPos = _ground.CellToWorld(data.Cell);
      if (_ground.GetCellInfo(data.Cell) is { IsWalkable: true }
          && WorldHasGrass(cellWorldPos)
          && !WorldHasAnotherSheep(cellWorldPos))
      {
        endData = data;
        break;
      }

      foreach (Vector3Int neighbour in GetNeighbours(data.Cell))
      {
        if (closedCells.FindIndex(neighbour.Equals) > -1) continue;

        closedCells.Add(neighbour);
        openCellsData.Enqueue(new PathCellData { Cell = neighbour, CameFrom = data });
      }
    }

    if (endData != null)
      _movementPath.Push(_ground.CellToWorld(endData.Cell));

    PathCellData currentData = endData;
    while (currentData?.CameFrom != null)
    {
      _movementPath.Push(_ground.CellToWorld(currentData.Cell));
      currentData = currentData.CameFrom;
    }

    IEnumerable<Vector3Int> GetNeighbours(Vector3Int cell)
    {
      Vector3Int[] offsets =
      {
        new(+0, +1), // up
        new(+1, +0), // right
        new(+0, -1), // down
        new(-1, +0), // left
      };

      foreach (Vector3Int offset in offsets)
      {
        Vector3Int neighbour = cell + offset;
        if (_ground.GetCellInfo(neighbour).IsWalkable
            && !WorldHasAnotherSheep(_ground.CellToWorld(neighbour)))
          yield return neighbour;
      }
    }
  }

  private static readonly Collider2D[] _overlaps = new Collider2D[10];

  private static bool WorldHasGrass(Vector2 point)
  {
    int overlapsCount = Physics2D.OverlapPointNonAlloc(point, _overlaps);

    for (int i = 0; i < overlapsCount; i++)
      if (_overlaps[i].HasComponent<Grass>())
        return true;

    return false;
  }

  private bool WorldHasAnotherSheep(Vector2 point)
  {
    int overlapsCount = Physics2D.OverlapPointNonAlloc(point, _overlaps);

    for (int i = 0; i < overlapsCount; i++)
    {
      Sheep sheep = _overlaps[i].GetComponent<Sheep>();
      if (sheep && sheep != this) return true;
    }

    return false;
  }

  public class PathCellData
  {
    public PathCellData CameFrom;
    public Vector3Int Cell;
  }

  private void ProcessMovement()
  {
    if (!_movementPath.Any()) return;

    float speed = _movementSpeed;

    Vector3 targetPosition = _movementPath.Peek();

    Vector3 scale = transform.localScale;
    scale.x = (targetPosition - transform.position).x switch
    {
      <= -0.1f => -1,
      >= 0.1f => 1,
      _ => scale.x
    };
    transform.localScale = scale;
    
    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
      _movementPath.Pop();
  }

  private void ProcessEating()
  {
    if (_movementPath.Any()) return;

    int overlapsCount = Physics2D.OverlapPointNonAlloc(transform.position, _overlaps);

    for (int i = 0; i < overlapsCount; i++)
    {
      Grass grass = _overlaps[i].GetComponent<Grass>();
      if (!grass) continue;
      Destroy(grass.gameObject);
    }
  }

  public void Release()
  {
    Destroy(gameObject);
  }
}