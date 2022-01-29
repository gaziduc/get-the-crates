using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private bool isExplosiveGun;
    
    private void OnDestroy()
    {
        GameObject go = GameObject.Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        go.GetComponent<AudioSource>().volume = LevelManager.instance.sfxVolume;
        
        if (isExplosiveGun)
            GameObject.FindWithTag("PlayerManager").GetComponent<CameraShake>().Shake(0.025f, 0.25f);
    }
}
