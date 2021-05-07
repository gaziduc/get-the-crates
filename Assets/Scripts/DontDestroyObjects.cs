using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyObjects : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;
        
    // Start is called before the first frame update
    void Start()
    {
        foreach (var obj in objects)
        {
            DontDestroyOnLoad(obj);
        }
    }
}
