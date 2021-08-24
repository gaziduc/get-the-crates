using UnityEngine;

public class SkyCloud : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 0.5f;
 
    // Position Storage Variables
    Vector3 posOffset;
    Vector3 tempPos;
    
    void Start () {
        posOffset = transform.position;
    }
    
    void Update () {
        // Float left/right with a Sin()
        tempPos = posOffset;
        tempPos.x += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;
 
        transform.position = tempPos;
    }
}
