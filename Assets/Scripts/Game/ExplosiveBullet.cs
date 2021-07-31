using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    
    private void OnDestroy()
    {
        GameObject go = GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        go.GetComponent<AudioSource>().volume = LevelManager.instance.sfxVolume;
    }
}
