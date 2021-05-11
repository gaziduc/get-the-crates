using Photon.Pun;
using UnityEngine;

public class InstantiatePlayerOnStart : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefabToInstantiate;
    
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(playerPrefabToInstantiate.name, new Vector3(Random.Range(-5f, 5f), 0, 0), Quaternion.identity);
    }
    
}
