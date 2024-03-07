using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Grass : MonoBehaviour
{
  [SerializeField] private float _respawnDelay = 2f;

  private GroundInfo _ground;
  private Grass _prototype;
  private readonly List<Grass> _instances = new();

  public bool IsPrototype => !_prototype;

  private void OnEnable()
  {
    if (IsPrototype)
      InitAsPrototype();
  }

  private void InitAsPrototype()
  {
    gameObject.hideFlags = HideFlags.HideInHierarchy;
    gameObject.SetActive(false);
    var ground = FindObjectOfType<GroundInfo>();
    if (!TrySpawnOnFreeCell(this, ground)) 
      Spawn(this, ground);
  }

  private void OnDestroy()
  {
    if (IsPrototype)
      foreach (Grass instance in _instances)
        Destroy(instance.gameObject);
    else
    {
      _prototype._instances.Remove(this);
      if (_ground)
        _ground.StartCoroutine(DelayedRespawn(_ground, _prototype, _respawnDelay));
    }
  }

  private static Grass Spawn(Grass prototype, GroundInfo ground)
  {
    Grass grass = Instantiate(prototype);
    prototype._instances.Add(grass);
    grass._prototype = prototype;
    grass.name = prototype.name;
    grass._ground = ground;
    grass.gameObject.SetActive(true);
    return grass;
  }

  public Grass SpawnOnFreeSpace()
  {
    return Instantiate(this);
  }

  private static IEnumerator DelayedRespawn(GroundInfo ground, Grass prototype, float delay)
  {
    while (true)
    {
      yield return new WaitForSeconds(delay);
      if (!prototype)
        yield break;
      if (TrySpawnOnFreeCell(prototype, ground))
        yield break;
    }
  }

  private static bool TrySpawnOnFreeCell(Grass prototype, GroundInfo ground)
  {
    if (!TryGetFreeCellPosition(ground, out Vector3 position)) return false;
    
    Spawn(prototype, ground).transform.position = position;
    return true;
  }

  private static readonly Random _random = new();

  private static bool TryGetFreeCellPosition(GroundInfo ground, out Vector3 cellPosition)
  {
    List<Vector3Int> cells = new();
    foreach (Vector3Int cell in ground.AllCells)
      if (IsFreeCell(ground.GetCellInfo(cell)))
        cells.Add(cell);

    if (cells.Count == 0)
    {
      cellPosition = default;
      return false;
    }

    cellPosition = ground.CellToWorld(cells[_random.Next(cells.Count)]);
    return true;

    bool IsFreeCell(GroundInfo.CellInfo info)
    {
      return info is { IsWalkable: true } && !WorldHasAnyGrass(info.WorldPos);
    }
  }

  private static readonly Collider2D[] _overlaps = new Collider2D[10];

  private static bool WorldHasAnyGrass(Vector2 point)
  {
    int overlapsCount = Physics2D.OverlapPointNonAlloc(point, _overlaps);

    for (int i = 0; i < overlapsCount; i++)
      if (_overlaps[i].HasComponent<Grass>())
        return true;

    return false;
  }

  public void Release() =>
    Destroy(gameObject);
}