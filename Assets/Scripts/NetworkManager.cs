using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte maxPlayersPerRoom = 2;
    [SerializeField] private string gameVersion = "0.3";
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private Dropdown roomsDropdown;
    [SerializeField] private Text currentRoomText;
    [SerializeField] private Text[] playerListTexts;
    [SerializeField] private InputField nicknameInputField;
    [SerializeField] private Button startButton;
    [SerializeField] private Text statusText;
    [SerializeField] private Button joinButton;
    
    private bool hasAvailableRooms;

    // Start is called before the first frame update
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        if (statusText && statusText.gameObject.activeInHierarchy)
            statusText.text = Regex.Replace(PhotonNetwork.NetworkClientState.ToString(), "([A-Z])", " $1");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        connectingPanel.SetActive(false);
        
        if (!PlayerPrefs.HasKey("Nickname"))
            nicknamePanel.SetActive(true);
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname");
            menuPanel.SetActive(true);
        }
    }

    public void SetNickname()
    {
        PhotonNetwork.NickName = nicknameInputField.text;
        PlayerPrefs.SetString("Nickname", PhotonNetwork.NickName);
        nicknamePanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomsDropdown.ClearOptions();

        if (roomList.Count == 0)
        {
            hasAvailableRooms = false;
            roomsDropdown.options.Add(new Dropdown.OptionData("No room available"));
            joinButton.interactable = false;
        }
        else
        {
            hasAvailableRooms = true;
            
            foreach (var room in roomList)
                roomsDropdown.options.Add(new Dropdown.OptionData(room.Name));

            joinButton.interactable = true;
        }
        
        roomsDropdown.RefreshShownValue();
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
        if (hasAvailableRooms)
        {
            PhotonNetwork.JoinRoom(roomsDropdown.options[roomsDropdown.value].text);
            
            menuPanel.SetActive(false);
            connectingPanel.SetActive(true);
        }
            
    }

    private void UpdatePlayerList()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
        {
            var player = PhotonNetwork.PlayerList[i];
            playerListTexts[i].text = "P" + (i + 1) + ": " + player.NickName;
            
            if (i == 1)
                playerListTexts[i].GetComponent<BlinkText>().DisableBlink();
        }

        for (int i = PhotonNetwork.CurrentRoom.Players.Count; i < maxPlayersPerRoom; i++)
        {
            playerListTexts[i].text = "P" + (i + 1) + ": Waiting for a player...";
            
            if (i == 1)
                playerListTexts[i].GetComponent<BlinkText>().EnableBlink();
        }
    }

    public override void OnJoinedRoom()
    {
        connectingPanel.SetActive(false);
        statusPanel.SetActive(true);

        // If master client, then able to launch game
        if (PhotonNetwork.IsMasterClient)
            startButton.interactable = true;

        currentRoomText.text = "Room name: " +  PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }
    
    public void LeaveGame()
    {
        statusPanel.SetActive(false);
        connectingPanel.SetActive(true);
        
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Level");
    }
}
