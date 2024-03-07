using System.Collections;
using UnityEngine;

public class CoroutineUpdateTest : MonoBehaviour
{
  [SerializeField] private int _iterationCount;
  
  private int _counter;
  
  private void OnEnable()
  {
    StartCoroutine(DoUpdate());
  }

  private IEnumerator DoUpdate()
  {
    while (enabled)
    {
      yield return null;
      
      for (int i = 0; i < _iterationCount; i++) 
        _counter++;
    }
  }
}