using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    private byte maxPlayersPerRoom = 4;
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private GameObject noRoomText;
    [SerializeField] private InputField currentRoomField;
    [SerializeField] private Text[] playerListTexts;
    [SerializeField] private InputField nicknameInputField;
    [SerializeField] private Button startButton;
    [SerializeField] private Text statusText;
    [SerializeField] private InputField chatInputField;
    [SerializeField] private GameObject viewPrefab;
    [SerializeField] private Text masterClientText;
    [SerializeField] private Text statsText;
    [SerializeField] private AudioSource playerEnter;
    [SerializeField] private AudioSource playerLeft;
    [SerializeField] private Text copyButtonText;
    [SerializeField] private InputField nicknameOptionsField;
    [SerializeField] private InputField roomToJoinInputField;
    [SerializeField] private Dropdown sizeDropdown;
    
    private Dictionary<string, RoomInfo> cachedRoomList;
    private PhotonView chatView;

    // Start is called before the first frame update
    private void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = Application.version;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname");
            nicknameOptionsField.text = PhotonNetwork.NickName;
            OnJoinedRoom();
        }
    }

    private void Update()
    {
        if (statusText && statusText.gameObject.activeInHierarchy)
            statusText.text = Regex.Replace(PhotonNetwork.NetworkClientState.ToString(), "([A-Z])", " $1");

        if (statsText && statsText.gameObject.activeInHierarchy)
            statsText.text = "Connected: " + PhotonNetwork.CountOfPlayers + " Player(s)\nIn a room: " +
                              PhotonNetwork.CountOfPlayersInRooms + " Player(s)";
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
            nicknameOptionsField.text = PhotonNetwork.NickName;
            menuPanel.SetActive(true);
        }
    }

    public void SetNickname(bool isInDialog)
    {
        string text = isInDialog ? nicknameInputField.text : nicknameOptionsField.text;
        
        if (!String.IsNullOrWhiteSpace(text))
        {
            PhotonNetwork.NickName = text;
            PlayerPrefs.SetString("Nickname", text);

            if (isInDialog)
            {
                nicknameOptionsField.text = text;
                nicknamePanel.SetActive(false);
                menuPanel.SetActive(true);
            }
            else if (PhotonNetwork.InRoom)
            {
                UpdatePlayerList();
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (!room.IsVisible || room.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(room.Name))
                    cachedRoomList.Remove(room.Name);

                continue;
            }

            if (cachedRoomList.ContainsKey(room.Name))
                cachedRoomList[room.Name] = room;
            else
                cachedRoomList.Add(room.Name, room);
        }

        
        for (int i = 0; i < content.transform.childCount; i++)
        {
            GameObject.Destroy(content.transform.GetChild(i).gameObject);
        }
        
        foreach (var room in cachedRoomList)
        {
            GameObject roomItem = GameObject.Instantiate(roomItemPrefab, content.transform);
            Text text = roomItem.transform.GetChild(0).GetComponent<Text>(); 
            text.text = room.Key + " <color=grey>(" + room.Value.PlayerCount + "/" + room.Value.MaxPlayers + ")</color>";

            if (!room.Value.IsOpen)
            {
                text.text += " <color=red>Playing</color>";
                Destroy(roomItem.transform.GetChild(1).gameObject);
            }
            else
                roomItem.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(room.Key); });
        }
        
        if (cachedRoomList.Count != 0 && noRoomText.activeSelf)
            noRoomText.SetActive(false);
        else if (cachedRoomList.Count == 0 && !noRoomText.activeSelf)
            noRoomText.SetActive(true);
            
    }

    public void CreateRoom()
    {
        menuPanel.SetActive(false);
        connectingPanel.SetActive(true);
        
        var roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        PhotonNetwork.CreateRoom(String.IsNullOrWhiteSpace(roomNameInputField.text) ? GetRandomString(): roomNameInputField.text, roomOptions, TypedLobby.Default);
    }

    private void JoinRoom(string roomName)
    {
        menuPanel.SetActive(false);
        connectingPanel.SetActive(true);
        
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRoomWithName()
    {
        if (!String.IsNullOrWhiteSpace(roomToJoinInputField.text))
        {
            menuPanel.SetActive(false);
            connectingPanel.SetActive(true);
        
            PhotonNetwork.JoinRoom(roomToJoinInputField.text);
        }
    }

    private void UpdatePlayerList()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Count; i++)
        {
            var player = PhotonNetwork.PlayerList[i];
            playerListTexts[i].text = "P" + (i + 1) + ": " + player.NickName;
            
            if (i > 0)
                playerListTexts[i].GetComponent<BlinkText>().DisableBlink();
        }

        for (int i = PhotonNetwork.CurrentRoom.Players.Count; i < maxPlayersPerRoom; i++)
        {
            playerListTexts[i].text = "P" + (i + 1) + ": Waiting...";
            
            if (i > 0)
                playerListTexts[i].GetComponent<BlinkText>().EnableBlink();
        }
    }

    public override void OnJoinedRoom()
    {
        connectingPanel.SetActive(false);
        statusPanel.SetActive(true);

        // If master client, then able to launch game
        startButton.interactable = PhotonNetwork.IsMasterClient;
        sizeDropdown.interactable = PhotonNetwork.IsMasterClient;
        
        currentRoomField.text = PhotonNetwork.CurrentRoom.Name;

        GameObject temp = PhotonNetwork.Instantiate(viewPrefab.name, Vector3.zero, Quaternion.identity);
        chatView = temp.GetComponent<PhotonView>();
        
        SetMasterClientText(PhotonNetwork.MasterClient.NickName);

        UpdatePlayerList();
    }

    private void SetMasterClientText(string masterClientName)
    {
        masterClientText.text = "Master Client: " + masterClientName;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetMasterClientText(newMasterClient.NickName);

        startButton.interactable = newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
        sizeDropdown.interactable = newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public void JoinRandom()
    {
        menuPanel.SetActive(false);
        connectingPanel.SetActive(true);

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerEnter.Play();
        chatView.GetComponent<Chat>().SendMessageRPC("<color=orange>"  + newPlayer.NickName + " joined the room.</color>");
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerLeft.Play();
        chatView.GetComponent<Chat>().SendMessageRPC("<color=orange>"  + otherPlayer.NickName + " left the room.</color>");
        UpdatePlayerList();
    }
    
    public void LeaveGame()
    {
        statusPanel.SetActive(false);
        connectingPanel.SetActive(true);
        
        chatView.GetComponent<Chat>().ClearMessages();
        
        PhotonNetwork.LeaveRoom();
        
        Destroy(chatView.gameObject);
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerPrefs.SetString("LastLevelSize", sizeDropdown.options[sizeDropdown.value].text);
            PlayerPrefs.Save();
            
            PhotonNetwork.LoadLevel(sizeDropdown.options[sizeDropdown.value].text);
        }
           
    }

    private string GetRandomString()
    {
        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
        string res = "";

        for (int i = 0; i < 6; i++)
        {
            res += glyphs[Random.Range(0, glyphs.Length)];
        }

        return res;
    }

    public void SendChatMessage()
    {
        if (!String.IsNullOrWhiteSpace(chatInputField.text))
        {
            chatView.RPC("SendMessageRPC", RpcTarget.All, "<color=cyan>["  + PhotonNetwork.NickName + "]</color> " + chatInputField.text);
            chatInputField.text = "";
        }
        
        chatInputField.Select();
    }

    public void CopyToClipboard()
    {
        currentRoomField.text.CopyToClipboard();
        copyButtonText.text = "Copied!";

        StartCoroutine(CopyCoroutine());
    }

    private IEnumerator CopyCoroutine()
    {
        yield return new WaitForSeconds(2f);
        copyButtonText.text = "Copy";
    }
}
