using Photon.Pun;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public enum WinCondition
    {
        GetMostCrates = 0,
        KillMostPlayers = 1,
    }

    public WinCondition winCondition;

    [SerializeField] private GameObject spawnManager;

    public bool gameStarted = false;

    public float sfxVolume;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        winCondition = (WinCondition) ((int) PhotonNetwork.CurrentRoom.CustomProperties["win"]);
        
        PhotonNetwork.Instantiate(spawnManager.name, Vector3.zero, Quaternion.identity);
        
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.3f);
    }
}
