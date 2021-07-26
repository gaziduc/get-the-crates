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
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        winCondition = (WinCondition) ((int) PhotonNetwork.CurrentRoom.CustomProperties["win"]);
    }
}
