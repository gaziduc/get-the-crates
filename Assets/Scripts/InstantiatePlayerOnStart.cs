using Photon.Pun;
using UnityEngine;

public class InstantiatePlayerOnStart : MonoBehaviour
{
    [SerializeField] private GameObject[] playerPrefabToInstantiate;
    [SerializeField] private GameObject cratePrefabToInstantiate;
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = PhotonNetwork.Instantiate(PhotonNetwork.IsMasterClient ? playerPrefabToInstantiate[0].name
                                                                        : playerPrefabToInstantiate[1].name, GetRespawnPosition(), Quaternion.identity);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate(cratePrefabToInstantiate.name, GetCrateNewPosition(), Quaternion.identity);
    }

    public void Respawn()
    {
        player.transform.position = GetRespawnPosition();
    }
    
    private Vector3 GetRespawnPosition()
    {
        return new Vector3(Random.Range(-8f, 8f), 0, 0);
    }

    public static Vector3 GetCrateNewPosition()
    {
        return new Vector3(Random.Range(-14f, 14f), 0, 0);
    }

    public void RestartGame()
    {
        PhotonNetwork.LoadLevel("Empty");
    }
    
}
