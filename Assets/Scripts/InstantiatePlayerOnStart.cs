using Photon.Pun;
using UnityEngine;

public class InstantiatePlayerOnStart : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefabToInstantiate;
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PhotonNetwork.Instantiate(playerPrefabToInstantiate.name, GetRespawnPosition(), Quaternion.identity);
    }

    public void Respawn()
    {
        player.transform.position = GetRespawnPosition();
    }
    
    private Vector3 GetRespawnPosition()
    {
        return new Vector3(Random.Range(-8f, 8f), 0, 0);
    }
    
}
