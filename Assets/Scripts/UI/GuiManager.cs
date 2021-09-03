using System;
using System.Collections;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GuiManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text secondsText;
    [SerializeField] private GameObject endPanel;
    public GameObject pausePanel;
    [SerializeField] private Button backToMenuButtonEnd;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private AudioSource end;
    [SerializeField] private Text countdownText;
    [SerializeField] private AudioSource countdownSound;
    [SerializeField] private AudioSource countdownGoSound;
    [SerializeField] private GameObject countdownEffect;
    [SerializeField] private Text scoreboardText;
    [SerializeField] private Text scoresText;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject backToRoomButton;
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private Text leaderboardText;
    [SerializeField] private Text leaderScoresText;
    [SerializeField] private GameObject leaderboardPanel;

    // default selected buttons (for keyboard and gamepad)
    [SerializeField] private GameObject pauseDefault;
    
    private Text[] infoTexts;

    private float gameDuration = 90f;
    private float countdownDuration = 3f;
    
    private float timeRemaining;
    private int timeLast = 0;
    private float countdownTime;
    private int countdownLast = 0;
    private float beginTime = 0;

    private bool beginned = false;
    private bool ended = false;
    
    private PhotonView chatView;

    void Start()
    {
        LeanTween.scale(transitionPanel, Vector3.zero, 0.4f).setEaseOutCubic();
        
        backToMenuButton.interactable = PhotonNetwork.IsMasterClient;
        backToMenuButtonEnd.interactable = PhotonNetwork.IsMasterClient;

        infoTexts = new Text[infoPanel.transform.childCount];
        for (int i = 0; i < infoTexts.Length; i++)
        {
            infoTexts[i] = infoPanel.transform.GetChild(i).GetComponent<Text>();
            infoTexts[i].text = "";
        }
    
        #if UNITY_WEBGL
            AddMessage("<color=lime>Please download the game to get voice chat.</color>");
        #else
            AddMessage("<color=lime>Press <color=cyan>Tab</color> to enable/disable voice chat.</color>");
        #endif

        GameObject[] chatViews = GameObject.FindGameObjectsWithTag("ChatView");

        foreach (GameObject v in chatViews)
        {
            if (v.GetComponent<PhotonView>().IsMine)
            {
                chatView = v.GetComponent<PhotonView>();
                break;
            }
        }
    }
    
    
    public void AddMessage(string message)
    {
        for (int i = 0; i < infoTexts.Length - 1; i++)
            infoTexts[i].text = infoTexts[i + 1].text;

        infoTexts[infoTexts.Length - 1].text = message; 
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        AddMessage("<color=cyan>" + otherPlayer.NickName + " <color=red>left the room.</color></color>");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        bool amIMaster = newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
        
        backToMenuButton.interactable = amIMaster;
        backToMenuButtonEnd.interactable = amIMaster;
    }


    private void SetTimeText()
    {
        int timeNow = (int) timeRemaining + 1;
        
        if (timeNow != timeLast)
        {
            NewTextAnim(secondsText, timeNow.ToString());            

            timeLast = timeNow;
        }
    }

    private void NewTextAnim(Text text, string newText)
    {
        text.transform.localScale = Vector3.zero;
        text.text = newText;
        LeanTween.scale(text.gameObject, Vector3.one, 0.2f).setEaseOutBack();
    }
    
    private void UpdateLeaderboard()
    {
        Player[] players = PhotonNetwork.PlayerList;
        Array.Sort(players, (p1, p2) => p2.GetScore().CompareTo(p1.GetScore()));
        string nicknameText = "";
        string scoreText = "";
        GetScoreBoardText(players, ref nicknameText, ref scoreText);
        leaderboardText.text = nicknameText;
        leaderScoresText.text = scoreText;
    }
    
    
    private void Update()
    {
        if (leaderboardPanel.activeInHierarchy)
            UpdateLeaderboard();
        
        if (!beginned)
        {
            if (FindObjectsOfType<PhotonView>().Length - 1 == PhotonNetwork.PlayerList.Length * 3) // - 1 for the crate
            {
                countdownTime = countdownDuration;
                beginTime = Time.time;
                beginned = true;
            }
            else
                return;
        }

        if (countdownTime > 0f)
        {
            countdownTime = beginTime + countdownDuration - Time.time;

            if (countdownTime <= 0f)
            {
                NewTextAnim(countdownText, "GO!");
                GameObject.Destroy(countdownText.gameObject, 0.5f);

                LevelManager.instance.gameStarted = true;

                beginTime = Time.time;
                timeRemaining = gameDuration;
                
                countdownGoSound.Play();
                GameObject.Instantiate(countdownEffect, Vector3.zero, Quaternion.identity);
                return;
            }


            int countdownNow = (int) countdownTime + 1;
            if (countdownNow != countdownLast)
            {
                NewTextAnim(countdownText, ((int) countdownTime + 1).ToString());
                
                countdownSound.Play();
                GameObject.Instantiate(countdownEffect, Vector3.zero, Quaternion.identity);

                countdownLast = countdownNow;
            }
            
            return;
        }
        
        if (!endPanel.activeInHierarchy)
        {
            timeRemaining = beginTime + 90f - Time.time;
            SetTimeText();

            if (!ended && timeRemaining <= 0f)
            {
                ended = true;
                End();
            }
            
            if (Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        if (pausePanel.activeInHierarchy)
            LeanTween.scale(pausePanel, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(OnCompleteResume);
        else
        {
            pausePanel.SetActive(true);
            pausePanel.transform.localScale = Vector3.zero;
            LeanTween.scale(pausePanel, Vector3.one, 0.2f).setEaseOutBack();
            
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseDefault);
        }
    }

    private void OnCompleteResume()
    {
        pausePanel.SetActive(false);
    }

    private void GetScoreBoardText(Player[] players, ref string nicknames, ref string scores)
    {
        for (int i = 0; i < players.Length - 1; i++)
        {
            nicknames += players[i].NickName + "\n";
            scores += players[i].GetScore() + "\n";
        }

        nicknames += players[players.Length - 1].NickName;
        scores += players[players.Length - 1].GetScore();
    }

    private void End()
    {
        // disable pause menu if enabled
        if (pausePanel.activeInHierarchy)
            Pause();

        LeanTween.scale(leaderboardPanel, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(() => leaderboardPanel.SetActive(false));
        
        NewTextAnim(secondsText, "0");
        
        Player[] players = PhotonNetwork.PlayerList;
        Array.Sort(players, (p1, p2) => p2.GetScore().CompareTo(p1.GetScore()));
        
        string nicknameText = "";
        string scoreText = "";
        GetScoreBoardText(players, ref nicknameText, ref scoreText);
        scoreboardText.text = nicknameText;
        scoresText.text = scoreText;
        
        end.Play();
        
        endPanel.SetActive(true);
        endPanel.transform.localScale = Vector3.zero;
        backToRoomButton.SetActive(false);
        LeanTween.scale(endPanel, Vector3.one, 0.2f).setEaseOutBack();

        if (PhotonNetwork.IsMasterClient)
        {
            string winner = players[0].NickName;
            for (int i = 1; i < players.Length; i++)
            {
                if (players[i].GetScore() == players[0].GetScore())
                    winner += ", " + players[i].NickName;
                else
                    break;
            }
            
            // Set room custom properties for lobby
            Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;
            if (props.ContainsKey("winner"))
                props["winner"] =  winner;
            else
                props.Add("winner",  winner);
            
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        
        
        StartCoroutine(ShowButton());
    }

    private IEnumerator ShowButton()
    {
        yield return new WaitForSeconds(2f);
        backToRoomButton.SetActive(true);
        backToRoomButton.transform.localScale = Vector3.zero;
        LeanTween.scale(backToRoomButton, Vector3.one, 0.2f).setEaseOutBack();
    }
    
    public void GoBackToMenu()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(BackCoroutine());
        }
    }

    private IEnumerator BackCoroutine()
    {
        // Start transition
        chatView.RPC("StartTransitionRPC", RpcTarget.All);
        
        // Destroy some network object
        PhotonNetwork.Destroy(GameObject.FindWithTag("Crate"));
        
        SpawnManager[] spawnManagers = GameObject.FindObjectsOfType<SpawnManager>();
        foreach (var s in spawnManagers)
            PhotonNetwork.Destroy(s.gameObject);

        PlayerMovement[] players = GameObject.FindObjectsOfType<PlayerMovement>();
        foreach (var p in players)
            PhotonNetwork.Destroy(p.gameObject);

        yield return new WaitForSeconds(0.5f);
        
        PhotonNetwork.CurrentRoom.IsVisible = true;
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.LoadLevel(0);
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        chatView.GetComponent<Chat>().SetVoiceStatus();
    }
}
