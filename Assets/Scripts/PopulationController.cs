using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.UI;

public class PopulationController : MonoBehaviour
{
  [SerializeField] private Slider _sheepCount;
  [SerializeField] private Slider _grassCount;
  [SerializeField] private Sheep _sheepPrototype;
  [SerializeField] private Grass _grassPrototype;

  private Ground _ground;

  private void Awake() =>
    _ground = _ground ? _ground : FindObjectOfType<Ground>();

  private void Start()
  {
    _grassCount.value = FindObjectsOfType<Grass>().Length;
    _sheepCount.value = FindObjectsOfType<Sheep>().Length;
  }

  public void UpdateGrassPopulation()
  {
    Grass[] grasses = FindObjectsOfType<Grass>(includeInactive: true)
      .Where(grass => grass.IsPrototype).ToArray();
    for (int i = 0; i < grasses.Length - _grassCount.value; i++)
      Destroy(grasses[i].gameObject);

    for (int i = 0; i < _grassCount.value - grasses.Length; i++)
    {
      Ground.Cell cellWithoutGrass = _ground.GetCells(CanPlaceGrass).Random();
      if (cellWithoutGrass.IsValid)
        Instantiate(_grassPrototype, cellWithoutGrass.WorldPos, Quaternion.identity);
      else
        Instantiate(_grassPrototype);
    }

    bool CanPlaceGrass(Ground.Cell cell) =>
      cell.IsWalkable && !cell.HasAnyGrass();
  }

  public void UpdateSheepPopulation()
  {
    Sheep[] sheepes = FindObjectsOfType<Sheep>();
    for (int i = 0; i < sheepes.Length - _sheepCount.value; i++)
      Destroy(sheepes[i].gameObject);

    for (int i = 0; i < _sheepCount.value - sheepes.Length; i++)
    {
      Ground.Cell cellWithoutSheep = _ground.GetCells(CanPlaceSheep).Random();
      if (cellWithoutSheep.IsValid)
        Instantiate(_sheepPrototype, cellWithoutSheep.WorldPos, Quaternion.identity);
      else
        Instantiate(_sheepPrototype);
    }

    bool CanPlaceSheep(Ground.Cell cell) =>
      cell.IsWalkable && !cell.HasAnySheep();
  }
}