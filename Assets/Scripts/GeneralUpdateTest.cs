using UnityEngine;

public class GeneralUpdateTest : MonoBehaviour
{
    [SerializeField] private int _iterationCount;

    int _counter;
    
    void Update()
    {
        _counter++;      
        
        for (int i = 0; i < _iterationCount; i++) 
            _counter++;
    }
}