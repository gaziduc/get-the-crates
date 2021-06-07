using Photon.Pun;
using UnityEngine;

public class InstantiatePlayerOnStart : MonoBehaviour
{
    [SerializeField] private GameObject[] playerPrefabToInstantiate;
    [SerializeField] private GameObject cratePrefabToInstantiate;
    private GameObject player;
    
    [SerializeField] private Transform spawnPositionsParent;
    private Vector3[] spawnPosition;
    
    [SerializeField] private Transform cratePositionsParent;
    private Vector3[] cratePosition;

    private Vector3 lastPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = new Vector3[spawnPositionsParent.childCount];
        for (int i = 0; i < spawnPosition.Length; i++)
            spawnPosition[i] = spawnPositionsParent.GetChild(i).position;
        
        cratePosition = new Vector3[cratePositionsParent.childCount];
        for (int i = 0; i < cratePosition.Length; i++)
            cratePosition[i] = cratePositionsParent.GetChild(i).position;

        player = PhotonNetwork.Instantiate(PhotonNetwork.IsMasterClient ? playerPrefabToInstantiate[0].name
                                                                        : playerPrefabToInstantiate[1].name, GetRespawnPosition(), Quaternion.identity);
        
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate(cratePrefabToInstantiate.name, GetCrateNewPosition(Vector3.zero), Quaternion.identity);
    }

    public void Respawn()
    {
        player.transform.position = GetRespawnPosition();
    }
    
    private Vector3 GetRespawnPosition()
    {
        return spawnPosition[Random.Range(0, spawnPosition.Length)];
    }

    public Vector3 GetCrateNewPosition(Vector3 lastPos)
    {
        Vector3 newPos;
        do
        {
            newPos = cratePosition[Random.Range(0, cratePosition.Length)];
        } while (Vector3.Distance(newPos, lastPos) < 2);

        return newPos;
    }

    public void RestartGame()
    {
        PhotonNetwork.LoadLevel("Empty");
    }
}
