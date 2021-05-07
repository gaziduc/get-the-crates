using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte maxPlayersPerRoom = 2;
    [SerializeField] private string gameVersion = "0.1";
    [SerializeField] private GameObject connectingText;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject playerPrefabToInstantiate;
    
    // Start is called before the first frame update
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void JoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnConnectedToMaster()
    {
        connectingText.SetActive(false);
        menu.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Hashtable customProps = new Hashtable();
        customProps["roomName"] = "Level";

        PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);
        
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }
    
    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefabToInstantiate.name,new Vector3(-5, 0, 0), Quaternion.identity);
        DontDestroyOnLoad(player);
        
        SceneManager.LoadScene((string) PhotonNetwork.LocalPlayer.CustomProperties["roomName"]);
    }
}
