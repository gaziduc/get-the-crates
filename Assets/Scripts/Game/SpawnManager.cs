using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private PhotonView view;

    [SerializeField] private GameObject[] playerPrefabToInstantiate;
    [SerializeField] private GameObject botPrefabToInstantiate;
    [SerializeField] private GameObject cratePrefabToInstantiate;
    private Transform spawnPositionsParent;
    private Vector3[] spawnPosition;
    private Transform cratePositionsParent;
    private Vector3[] cratePosition;

    public static SpawnManager instance;
    
    
    void Start()
    {
        // Singleton
        instance = this;
        
        view = GetComponent<PhotonView>();

        spawnPositionsParent = GameObject.FindWithTag("PlayerSpawnParent").transform;
        
        spawnPosition = new Vector3[spawnPositionsParent.childCount];
        for (int i = 0; i < spawnPosition.Length; i++)
            spawnPosition[i] = spawnPositionsParent.GetChild(i).position;
        
        cratePositionsParent = GameObject.FindWithTag("CrateSpawnParent").transform;
        
        cratePosition = new Vector3[cratePositionsParent.childCount];
        for (int i = 0; i < cratePosition.Length; i++)
            cratePosition[i] = cratePositionsParent.GetChild(i).position;
        
        bool[] takenSpawn = new bool[spawnPositionsParent.childCount];
        for (int i = 0; i < takenSpawn.Length; i++)
            takenSpawn[i] = false;
        
        if (PhotonNetwork.IsMasterClient && view.IsMine)
        {
            // Players
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                int spawnNum = GetSpawnNum(takenSpawn);
                takenSpawn[spawnNum] = true;
                view.RPC("InstantiatePlayerRPC", p, spawnNum);
            }

            int numBots = 0;
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("bots"))
                numBots = (int) PhotonNetwork.CurrentRoom.CustomProperties["bots"];
            
            // Bots
            for (int i = 0; i < numBots; i++)
            {
                int spawnNum = GetSpawnNum(takenSpawn);
                takenSpawn[spawnNum] = true;
                StartCoroutine(InstantiateBot(spawnNum));
            }
            
            
            // Crate
            PhotonNetwork.Instantiate(cratePrefabToInstantiate.name, GetCrateNewPosition(Vector3.zero), Quaternion.identity);
        }
    }

    private int GetSpawnNum(bool[] takenSpawn)
    {
        int spawnNum = Random.Range(0, takenSpawn.Length);
        while (takenSpawn[spawnNum])
        {
            spawnNum++;
            if (spawnNum >= takenSpawn.Length)
                spawnNum = 0;
        }

        return spawnNum;
    }
    
    public Vector3 GetRespawnPos()
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
    

    [PunRPC]
    void InstantiatePlayerRPC(int index)
    {
        StartCoroutine(InstantiatePlayer(index));
    }

    private IEnumerator InstantiatePlayer(int index)
    {
        bool instantiated = false;
        
        while (!instantiated)
        {
            if (spawnPosition != null && spawnPosition[index] != Vector3.zero)
            {
                int skinNum = PlayerPrefs.GetInt("SkinNum", 0);
                if (skinNum >= playerPrefabToInstantiate.Length)
                    skinNum = 0;
                
                PhotonNetwork.Instantiate(playerPrefabToInstantiate[skinNum].name, spawnPosition[index], Quaternion.identity);
                
                instantiated = true;
            }
            
            yield return null;
        }
    }
    
    private IEnumerator InstantiateBot(int index)
    {
        bool instantiated = false;
        
        while (!instantiated)
        {
            if (spawnPosition != null && spawnPosition[index] != Vector3.zero)
            {
                PhotonNetwork.Instantiate(botPrefabToInstantiate.name, spawnPosition[index], Quaternion.identity);
                instantiated = true;
            }
            
            yield return null;
        }
    }
}
