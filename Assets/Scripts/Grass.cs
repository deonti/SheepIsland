using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class Grass : MonoBehaviour
{
  [SerializeField] private float _respawnDelay = 2f;

  private Ground _ground;
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
    Spawn(this, FindObjectOfType<Ground>());
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

  private static Grass Spawn(Grass prototype, Ground ground)
  {
    Grass grass = Instantiate(prototype);
    prototype._instances.Add(grass);
    grass._prototype = prototype;
    grass.name = prototype.name;
    grass._ground = ground;
    grass.gameObject.SetActive(true);
    return grass;
  }

  private static IEnumerator DelayedRespawn(Ground ground, Grass prototype, float delay)
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

  private static bool TrySpawnOnFreeCell(Grass prototype, Ground ground)
  {
    Ground.Cell freeCell = ground.GetCells(CellCanSpawnGrass).Random();
    if (!freeCell.IsValid) return false;

    Spawn(prototype, ground).transform.position = freeCell.WorldPos;
    return true;

    bool CellCanSpawnGrass(Ground.Cell cell) =>
      cell.IsWalkable && !cell.HasAnyGrass();
  }
}

public static partial class CellExtensions
{
  private static readonly Collider2D[] _grassOverlaps = new Collider2D[42];

  public static bool HasAnyGrass(this Ground.Cell cell)
  {
    int overlapsCount = Physics2D.OverlapPointNonAlloc(cell.WorldPos, _grassOverlaps);

    for (int i = 0; i < overlapsCount; i++)
      if (_grassOverlaps[i].HasComponent<Grass>())
        return true;

    return false;
  }
}