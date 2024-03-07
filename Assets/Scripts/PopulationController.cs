using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopulationController : MonoBehaviour
{
  [SerializeField] private Slider _sheepCount;
  [SerializeField] private Slider _grassCount;
  [SerializeField] private Sheep _sheepPrototype;
  [SerializeField] private Grass _grassPrototype;

  private GroundInfo _ground;

  private void Awake() => 
    _ground = _ground ? _ground : FindObjectOfType<GroundInfo>();

  private void Start() => 
    CountsChanged();

  public void CountsChanged()
  {
    Sheep[] sheepes = FindObjectsByType<Sheep>(FindObjectsSortMode.None);
    for (int i = sheepes.Length - 1; i > _sheepCount.value - 1; i--)
      sheepes[i].Release();

    for (int i = sheepes.Length; i < _sheepCount.value; i++)
      Instantiate(_sheepPrototype);

    Grass[] grasses = FindObjectsByType<Grass>(FindObjectsInactive.Include, FindObjectsSortMode.None)
      .Where(grass => grass.IsPrototype).ToArray();
    for (int i = grasses.Length - 1; i > _grassCount.value - 1; i--)
      grasses[i].Release();

    for (int i = grasses.Length; i < _grassCount.value; i++)
      Instantiate(_grassPrototype);
  }
}