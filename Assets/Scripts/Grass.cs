using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Grass : MonoBehaviour
{
  [SerializeField] private float _respawnDelay = 2f;

  private GroundInfo _ground;
  private Grass _prototype;

  private bool IsPrototype => !_prototype;

  private void Start()
  {
    if (IsPrototype)
      InitPrototype();
  }

  private void InitPrototype()
  {
    gameObject.hideFlags = HideFlags.HideInHierarchy;
    gameObject.SetActive(false);
    Spawn(this, FindObjectOfType<GroundInfo>());
  }

  private void OnDestroy()
  {
    if (_ground)
      _ground.StartCoroutine(DelayedRespawn(_ground, _prototype, _respawnDelay));
  }

  private static Grass Spawn(Grass prototype, GroundInfo ground)
  {
    Grass grass = Instantiate(prototype);
    grass.name = prototype.name;
    grass._prototype = prototype;
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
    yield return new WaitForSeconds(delay);
    Respawn(prototype, ground);
  }

  private static void Respawn(Grass prototype, GroundInfo ground)
  {
    if (prototype && TryGetFreeCellPosition(ground, out Vector3 position))
      Spawn(prototype, ground).transform.position = position;
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

  public void Release()
  {
    _ground = null;
    Destroy(gameObject);
  }
}