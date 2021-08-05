using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using FriendInfo = Photon.Realtime.FriendInfo;
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
    [SerializeField] private Text nicknameText;
    [SerializeField] private Button startButton;
    [SerializeField] private Text statusText;
    [SerializeField] private InputField chatInputField;
    [SerializeField] private GameObject viewPrefab;
    [SerializeField] private Text masterClientText;
    [SerializeField] private Text statsText;
    [SerializeField] private AudioSource playerEnter;
    [SerializeField] private AudioSource playerLeft;
    [SerializeField] private Text copyButtonText;
    [SerializeField] private InputField roomToJoinInputField;
    [SerializeField] private Dropdown sizeDropdown;
    [SerializeField] private Dropdown winConditionDropdown;
    [SerializeField] private GameObject disconnectedPanel;
    [SerializeField] private AudioSource selectSound;
    [SerializeField] private AudioSource backSound;
    [SerializeField] private InputField loginUser;
    [SerializeField] private InputField loginPass;
    [SerializeField] private InputField registerUser;
    [SerializeField] private InputField registerPass;
    [SerializeField] private InputField registerEmail;
    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Toggle savePasswordToggle;
    [SerializeField] private GameObject friendsContent;
    [SerializeField] private GameObject friendItemPrefab;
    [SerializeField] private GameObject noFriendText;
    [SerializeField] private InputField friendNameField;
    [SerializeField] private GameObject friendErrorGameobject;
    
    private string playerIdCache = "";
    private string username = "";
    private string password = "";
    
    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, string> cachedFriendList;
    private PhotonView chatView;

    private float UIAnimDelay = 0.2f;
    private float lastFriendsUpdateTime = 0f;

    // Start is called before the first frame update
    private void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = Application.version;

            loginUser.text = PlayerPrefs.GetString("Nickname", "");
            if (PlayerPrefs.GetInt("SavePassword", 1) == 1)
            {
                savePasswordToggle.isOn = true;
                loginPass.text = PlayerPrefs.GetString("Password", "");
            }
            
            ActivateUIElement(nicknamePanel);
        }
        else
        {
            nicknameText.text = PhotonNetwork.NickName;
            OnJoinedRoom();
        }
    }


    public void Login()
    {
        selectSound.Play();
        LeanTween.scale(nicknamePanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteLogin);
    }

    private void OnCompleteLogin()
    {
        ActivateUIElement(connectingPanel);
        
        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
        request.Username = loginUser.text;
        request.Password = loginPass.text;

        username = loginUser.text;
        password = loginPass.text;
        
        PlayFabClientAPI.LoginWithPlayFab(request, RequestToken, PlayFabError);
    }

    public void Register()
    {
        selectSound.Play();
        LeanTween.scale(nicknamePanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteRegister);
    }

    private void OnCompleteRegister()
    {
        ActivateUIElement(connectingPanel);
        
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
        request.Username = registerUser.text;
        request.Password = registerPass.text;
        request.Email = registerEmail.text;

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterLogin, PlayFabError);
    }

    void OnRegisterLogin(RegisterPlayFabUserResult result)
    {
        LoginWithPlayFabRequest request = new LoginWithPlayFabRequest();
        request.Username = registerUser.text;
        request.Password = registerPass.text;
        
        username = registerUser.text;
        password = registerPass.text;
        
        PlayFabClientAPI.LoginWithPlayFab(request, RequestToken, PlayFabError);
    }
    
    
    void RequestToken(LoginResult result)
    {
        playerIdCache = result.PlayFabId;

        GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest();
        request.PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
        PlayFabClientAPI.GetPhotonAuthenticationToken(request, AuthWithPhoton, PlayFabError);
    }
    
    void AuthWithPhoton(GetPhotonAuthenticationTokenResult result)
    {
        var customAuth = new AuthenticationValues {AuthType = CustomAuthenticationType.Custom};
        customAuth.AddAuthParameter("username", playerIdCache);
        customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);

        PhotonNetwork.AuthValues = customAuth;

        PhotonNetwork.NickName = username;
        nicknameText.text = username;
        
        PlayerPrefs.SetString("Nickname", username);
        if (PlayerPrefs.GetInt("SavePassword", 1) == 1)
            PlayerPrefs.SetString("Password", password);
        else
            PlayerPrefs.SetString("Password", "");
        
        Connect();
    }


    void PlayFabError(PlayFabError error)
    {
        errorPanel.transform.localScale = Vector3.zero;
        errorPanel.SetActive(true);
        errorPanel.transform.GetChild(1).GetComponent<Text>().text = error.ToString();
        
        LeanTween.scale(connectingPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteError);
    }

    void OnCompleteError()
    {
        ActivateUIElement(errorPanel);
    }

    public void ReturnToLoginPanel()
    {
        selectSound.Play();
        LeanTween.scale(errorPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteReturnToLoginPanel);
    }

    void OnCompleteReturnToLoginPanel()
    {
        ActivateUIElement(nicknamePanel);
    }

    public void OnToggleSavePassword()
    {
        PlayerPrefs.SetInt("SavePassword", savePasswordToggle.isOn ? 1 : 0);
    }
    
    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void Update()
    {
        if (statusText && statusText.gameObject.activeInHierarchy)
            statusText.text = Regex.Replace(PhotonNetwork.NetworkClientState.ToString(), "([A-Z])", " $1");

        if (statsText && statsText.gameObject.activeInHierarchy)
            statsText.text = "Connected: " + PhotonNetwork.CountOfPlayers + " Player(s)\nIn a room: " +
                              PhotonNetwork.CountOfPlayersInRooms + " Player(s)";

        if (menuPanel.activeInHierarchy && lastFriendsUpdateTime + 8f < Time.time)
        {
            lastFriendsUpdateTime = Time.time;

            if (cachedFriendList.Count > 0)
            {
                string[] friendsToFind = new string[cachedFriendList.Count];

                int i = 0;
                foreach (var friend in cachedFriendList)
                {
                    friendsToFind[i] = friend.Key;
                    i++;
                }
         
                PhotonNetwork.FindFriends(friendsToFind);
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest() { ProfileConstraints = null }, FriendListResult, OnFriendListError);
        
        PhotonNetwork.JoinLobby();
    }
    
    private void FriendListResult(GetFriendsListResult result)
    {
        cachedFriendList = new Dictionary<string, string>();
        
        if (result.Friends.Count > 0)
        {
            string[] friendsToFind = new string[result.Friends.Count];

            for (int i = 0; i < result.Friends.Count; i++) {
                friendsToFind[i] = result.Friends[i].FriendPlayFabId;
                cachedFriendList.Add(friendsToFind[i], result.Friends[i].Username);
            }
        
            PhotonNetwork.FindFriends(friendsToFind); 
        }
    }

    void OnFriendListError(PlayFabError error)
    {
        friendErrorGameobject.transform.localScale = Vector3.zero;
        friendErrorGameobject.SetActive(true);
        friendErrorGameobject.transform.GetChild(1).GetComponent<Text>().text = error.ToString();
        
        ActivateUIElement(friendErrorGameobject);
    }

    public void OnFriendErrorOK()
    {
        LeanTween.scale(friendErrorGameobject, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteFriendErrorOK);
    }

    private void OnCompleteFriendErrorOK()
    {
        friendErrorGameobject.SetActive(false);
    }


    public void AddFriend()
    {
        AddFriendRequest request = new AddFriendRequest();
        request.FriendUsername = friendNameField.text;
        friendNameField.text = "";
        
        PlayFabClientAPI.AddFriend(request, OnAddedFriend, OnFriendListError);
    }

    void OnAddedFriend(AddFriendResult result)
    {
        if (result.Created)
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest() { ProfileConstraints = null }, FriendListResult, OnFriendListError);
        }
    }
    
    

    public override void OnJoinedLobby()
    {
        LeanTween.scale(connectingPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteJoinedLobby);
    }

    private void ActivateUIElement(GameObject objectToActivate)
    {
        objectToActivate.SetActive(true);
        objectToActivate.transform.localScale = Vector3.zero;
        LeanTween.scale(objectToActivate, Vector3.one, UIAnimDelay).setEaseOutBack();
    }
    
    private void OnCompleteJoinedLobby()
    {
        connectingPanel.SetActive(false);

        ActivateUIElement(menuPanel);
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

        
        for (int i = content.transform.childCount - 1; i >= 0; i--)
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

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        for (int i = friendsContent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(friendsContent.transform.GetChild(i).gameObject);
        }
        
        if (friendList.Count == 0 && !noFriendText.activeSelf)
            noFriendText.SetActive(true);
        else if (friendList.Count != 0 && noFriendText.activeSelf)
            noFriendText.SetActive(false);
        
        foreach (FriendInfo friend in friendList)
        {
            if (friend.IsOnline)
            {
                GameObject friendItem = GameObject.Instantiate(friendItemPrefab, friendsContent.transform);
                Text text = friendItem.transform.GetChild(0).GetComponent<Text>();
                text.text = cachedFriendList[friend.UserId] + " <color=green>Online</color>" + (friend.IsInRoom ? " in room: " + friend.Room : " in lobby");

                if (friend.IsInRoom)
                    friendItem.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(friend.Room); });
                else
                    Destroy(friendItem.transform.GetChild(1).gameObject);
            }
        }
        
        foreach (FriendInfo friend in friendList)
        {
            if (!friend.IsOnline)
            {
                GameObject friendItem = GameObject.Instantiate(friendItemPrefab, friendsContent.transform);
                Text text = friendItem.transform.GetChild(0).GetComponent<Text>();
                text.text = cachedFriendList[friend.UserId] + " <color=grey>Offline</color>";
                Destroy(friendItem.transform.GetChild(1).gameObject);
            }
        }
        
        
    }

    public void CreateRoom()
    {
        selectSound.Play();
        LeanTween.scale(menuPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteCreateRoom);
    }

    private void OnCompleteCreateRoom()
    {
        menuPanel.SetActive(false);
        
        if (!connectingPanel.activeInHierarchy)
            ActivateUIElement(connectingPanel);
        
        statusPanel.SetActive(true);
        statusPanel.transform.localScale = Vector3.zero;
        
        var roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        roomOptions.PublishUserId = true; // for friends
        PhotonNetwork.CreateRoom(String.IsNullOrWhiteSpace(roomNameInputField.text) ? GetRandomString(): roomNameInputField.text, roomOptions, TypedLobby.Default);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        LeanTween.scale(connectingPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteOnCreateRoomFailed);
    }

    private void OnCompleteOnCreateRoomFailed()
    {
        connectingPanel.SetActive(false);
        ActivateUIElement(menuPanel);
    }

    private void JoinRoom(string roomName)
    {
        selectSound.Play();
        LeanTween.scale(menuPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteJoinRoom, roomName);
    }

    private void OnCompleteJoinRoom(object roomName)
    {
        menuPanel.SetActive(false);
        ActivateUIElement(connectingPanel);
        
        statusPanel.SetActive(true);
        statusPanel.transform.localScale = Vector3.zero;
        
        PhotonNetwork.JoinRoom((string) roomName);
    }

    public void JoinRoomWithName()
    {
        if (!String.IsNullOrWhiteSpace(roomToJoinInputField.text))
        {
            selectSound.Play();
            LeanTween.scale(menuPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteJoinRoomWithName);
        }
    }

    private void OnCompleteJoinRoomWithName()
    {
        menuPanel.SetActive(false);
        ActivateUIElement(connectingPanel);
        
        statusPanel.SetActive(true);
        statusPanel.transform.localScale = Vector3.zero;

        PhotonNetwork.JoinRoom(roomToJoinInputField.text);
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
        
        if (chatView) // Do not remove, check because SetNickname can call this method before chatView is gotten
            chatView.GetComponent<Chat>().ResetVoiceStatusForRemainingPlayers();
    }

    public override void OnJoinedRoom()
    {
        LeanTween.scale(connectingPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteOnJoinedRoom);
    }

    private void OnCompleteOnJoinedRoom()
    {
        connectingPanel.SetActive(false);
        ActivateUIElement(statusPanel);

        // If master client, then able to launch game
        startButton.interactable = PhotonNetwork.IsMasterClient;
        sizeDropdown.interactable = PhotonNetwork.IsMasterClient;
        winConditionDropdown.interactable = PhotonNetwork.IsMasterClient;

        sizeDropdown.value = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("level") ? (int) PhotonNetwork.CurrentRoom.CustomProperties["level"] : 0;
        winConditionDropdown.value = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("win") ? (int) PhotonNetwork.CurrentRoom.CustomProperties["win"] : 0;
        
        currentRoomField.text = PhotonNetwork.CurrentRoom.Name;

        PhotonView[] views = GameObject.FindObjectsOfType<PhotonView>();

        foreach (PhotonView v in views)
        {
            if (v.IsMine)
            {
                chatView = v;
                break;
            }
        }
        
        if (chatView == null)
        {
            GameObject temp = PhotonNetwork.Instantiate(viewPrefab.name, Vector3.zero, Quaternion.identity);
            chatView = temp.GetComponent<PhotonView>();
        }


        ApplyVoiceVolume(PlayerPrefs.GetFloat("VoiceVolume", 0.8f));
        
        SetMasterClientText(PhotonNetwork.MasterClient.NickName);

        SetProperties();
        
        UpdatePlayerList();
    }
    
    private void SetMasterClientText(string masterClientName)
    {
        masterClientText.text = "Master Client: " + masterClientName;
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LeanTween.scale(connectingPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteOnJoinRoomFailed);
    }

    private void OnCompleteOnJoinRoomFailed()
    {
        connectingPanel.SetActive(false);
        ActivateUIElement(menuPanel);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetMasterClientText(newMasterClient.NickName);

        bool amIMaster = newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
        
        startButton.interactable = amIMaster;
        sizeDropdown.interactable = amIMaster;
        winConditionDropdown.interactable = amIMaster;
    }

    public void JoinRandom()
    {
        selectSound.Play();
        LeanTween.scale(menuPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteJoinRandom);
    }
    
    private void OnCompleteJoinRandom()
    {
        menuPanel.SetActive(false);
        ActivateUIElement(connectingPanel);
        
        statusPanel.SetActive(true);
        statusPanel.transform.localScale = Vector3.zero;

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // Create room
        LeanTween.scale(menuPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteCreateRoom);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerEnter.Play();
        chatView.GetComponent<Chat>().SendMessageRPC("<color=cyan>"  + newPlayer.NickName + "</color> <color=orange>joined the room.</color>");

        if (PhotonNetwork.IsMasterClient)
        {
            chatView.RPC("SetLevelDropdownStateRPC", RpcTarget.Others, sizeDropdown.value);
            chatView.RPC("SetWinConditionStateRPC", RpcTarget.Others, winConditionDropdown.value);
        }
        
        UpdatePlayerList();
        
        // Debug.LogError(PhotonNetwork.PlayerList[0].CustomProperties.ToString());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        playerLeft.Play();
        chatView.GetComponent<Chat>().SendMessageRPC("<color=cyan>"  + otherPlayer.NickName + "</color> <color=orange>left the room.</color>");
        UpdatePlayerList();
        
        chatView.GetComponent<Chat>().SetVoiceStatus();
    }
    
    public void LeaveGame()
    {
        backSound.Play();
        
        LeanTween.scale(statusPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteLeaveGame);
    }

    private void OnCompleteLeaveGame()
    {
        chatView.GetComponent<Chat>().ClearMessages();
        
        statusPanel.SetActive(false);
        ActivateUIElement(connectingPanel);
        
        PhotonNetwork.Destroy(chatView);
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            selectSound.Play();
            
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(sizeDropdown.options[sizeDropdown.value].text);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        menuPanel.SetActive(false);
        connectingPanel.SetActive(false);
        statusPanel.SetActive(false);

        ActivateUIElement(disconnectedPanel);

        disconnectedPanel.transform.GetChild(1).GetComponent<Text>().text = cause.ToString();
    }

    public void TryReconnect()
    {
        selectSound.Play();
        LeanTween.scale(disconnectedPanel, Vector3.zero, UIAnimDelay).setEaseInBack().setOnComplete(OnCompleteTryReconnect);
    }

    private void OnCompleteTryReconnect()
    {
        disconnectedPanel.SetActive(false);
        Connect();
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
        string text = Regex.Replace(chatInputField.text, "<.*?>", string.Empty);
        
        if (!String.IsNullOrWhiteSpace(text))
        {
            chatView.RPC("SendMessageRPC", RpcTarget.All, "<color=cyan>["  + PhotonNetwork.NickName + "]</color> " + text);
            chatInputField.text = "";
        }
        
        chatInputField.Select();
    }

    public void SendLevelSize()
    {
        if (sizeDropdown.interactable)
        {
            SetProperties();

            if (chatView)
            {
                chatView.RPC("SendMessageRPC", RpcTarget.All, "<color=lime>Master Client <color=cyan>" + PhotonNetwork.MasterClient.NickName + "</color> set level size to <color=orange>" + sizeDropdown.options[sizeDropdown.value].text + "</color></color>");
                chatView.RPC("SetLevelDropdownStateRPC", RpcTarget.Others, sizeDropdown.value);
            }
        }
    }

    public void SendWinCondition()
    {
        if (winConditionDropdown.interactable)
        {
            SetProperties();

            if (chatView)
            {
                chatView.RPC("SendMessageRPC", RpcTarget.All, "<color=lime>Master Client <color=cyan>" + PhotonNetwork.MasterClient.NickName + "</color> set win condition to <color=orange>" + winConditionDropdown.options[winConditionDropdown.value].text + "</color></color>");
                chatView.RPC("SetWinConditionStateRPC", RpcTarget.Others, winConditionDropdown.value);
            }
        }
    }

    public void ToggleVoice()
    {
        chatView.GetComponent<Chat>().ToggleVoiceChat();
    }

    public void ApplyVoiceVolume(float volume)
    {
        if (chatView)
            chatView.GetComponent<AudioSource>().volume = volume;
    }

    private void SetProperties()
    {
        Hashtable hashtable = new Hashtable();
        hashtable.Add("level", sizeDropdown.value);
        hashtable.Add("win", winConditionDropdown.value);
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
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

    private IEnumerator WaitForChatView()
    {
        while (chatView == null)
            yield return null;
        
        chatView.GetComponent<Chat>().SetVoiceStatus();
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        StartCoroutine(WaitForChatView());
    }
}
