using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte maxPlayersPerRoom = 2;
    [SerializeField] private string gameVersion = "0.1";
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private GameObject playerPrefabToInstantiate;
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private Dropdown roomsDropdown;
    [SerializeField] private Text currentRoomText;
    [SerializeField] private Text[] playerListTexts;
    [SerializeField] private InputField nicknameInputField;

    private string nickname;

    // Start is called before the first frame update
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        connectingPanel.SetActive(false);
        nicknamePanel.SetActive(true);
    }

    public void SetNickname()
    {
        PhotonNetwork.NickName = nicknameInputField.text;
        nicknamePanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomsDropdown.ClearOptions();
        
        foreach (var room in roomList)
            roomsDropdown.options.Add(new Dropdown.OptionData(room.Name));
    }

    public void CreateRoom()
    {
        var roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions, TypedLobby.Default);

        menuPanel.SetActive(false);
        connectingPanel.SetActive(true);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomsDropdown.options[roomsDropdown.value].text);
    }

    private void UpdatePlayerList()
    {
        for (int i = 0; i < maxPlayersPerRoom && i < PhotonNetwork.CurrentRoom.Players.Count; i++)
        {
            var player = PhotonNetwork.PlayerList[i];
            playerListTexts[i].text = "P" + (i + 1) + ": " + (player != null ? player.NickName : "No player");
        }
    }

    public override void OnJoinedRoom()
    {
        connectingPanel.SetActive(false);
        statusPanel.SetActive(true);
        
        currentRoomText.text = "Room name: " +  PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Level");

        StartCoroutine(InstanciatePlayer());
    }

    private IEnumerator InstanciatePlayer()
    {
        while (PhotonNetwork.LevelLoadingProgress < 1f)
            yield return new WaitForSeconds(0.02f);
        
        PhotonNetwork.Instantiate(playerPrefabToInstantiate.name, new Vector3(-5, 0, 0), Quaternion.identity);
    }
}
