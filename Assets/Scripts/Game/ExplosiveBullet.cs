using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    
    private void OnDestroy()
    {
        GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}
