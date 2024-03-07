using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulationController : MonoBehaviour
{
  [SerializeField] private Slider _sheepCount;
  [SerializeField] private Slider _grassCount;
  [SerializeField] private Sheep _sheepPrototype;
  [SerializeField] private Grass _grassPrototype;

  private readonly List<Sheep> _sheepList = new();
  private readonly List<Grass> _grassList = new();
  private GroundInfo _ground;

  private void Awake()
  {
    _ground = _ground ? _ground : FindObjectOfType<GroundInfo>();
  }

  private void Start()
  {
    CountsChanged();
  }

  public void CountsChanged()
  {
    _sheepList.ForEach(sheep => sheep.Release());
    _sheepList.Clear();

    _grassList.ForEach(grass => grass.Release());
    _grassList.Clear();

    foreach (Sheep sheep in FindObjectsByType<Sheep>(FindObjectsSortMode.None))
      sheep.Release();

    foreach (Grass grass in FindObjectsByType<Grass>(FindObjectsSortMode.None))
      grass.Release();

    for (int i = 0; i < _grassCount.value; i++)
      _grassList.Add(_grassPrototype.SpawnOnFreeSpace());

    for (int i = 0; i < _sheepCount.value; i++)
      _sheepList.Add(Instantiate(_sheepPrototype));
  }
}