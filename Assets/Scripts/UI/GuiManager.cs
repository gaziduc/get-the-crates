using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Slider countdownSlider;
    [SerializeField] private GameObject centerPanel;
    [SerializeField] private Text thirtySecRemainingText;
    [SerializeField] private Text tenSecRemainingText;
    [SerializeField] private AudioSource secRemainingSound;
    
    // default selected buttons (for keyboard and gamepad)
    [SerializeField] private GameObject pauseDefault;
    
    private Text[] infoTexts;

    private float gameDuration = 100f;
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

            if (timeNow == 30)
            {
                thirtySecRemainingText.gameObject.SetActive(true);
                thirtySecRemainingText.transform.localScale = Vector3.zero;
                secRemainingSound.Play();
                LeanTween.scale(thirtySecRemainingText.gameObject, Vector3.one, 0.2f).setEaseOutBack().setOnComplete(OnComplete30Sec);
            }
            else if (timeNow == 10)
            {
                tenSecRemainingText.gameObject.SetActive(true);
                tenSecRemainingText.transform.localScale = Vector3.zero;
                secRemainingSound.Play();
                LeanTween.scale(tenSecRemainingText.gameObject, Vector3.one, 0.2f).setEaseOutBack().setOnComplete(OnComplete10Sec);
            }
            
            timeLast = timeNow;
        }
    }

    private void OnComplete10Sec()
    {
        StartCoroutine(HideTextCoroutine(tenSecRemainingText));
    }
    
    private void OnComplete30Sec()
    {
        StartCoroutine(HideTextCoroutine(thirtySecRemainingText));
    }

    private IEnumerator HideTextCoroutine(Text text)
    {
        yield return new WaitForSeconds(1.6f);
        LeanTween.scale(text.gameObject, Vector3.zero, 0.2f).setEaseInBack();
        yield return new WaitForSeconds(0.2f);
        text.gameObject.SetActive(false);
    }

    private void NewTextAnim(Text text, string newText)
    {
        text.transform.localScale = Vector3.zero;
        text.text = newText;
        LeanTween.scale(text.gameObject, Vector3.one, 0.2f).setEaseOutBack();
        countdownSlider.value = 0;
    }
    
    private void UpdateLeaderboard()
    {
        Player[] players = PhotonNetwork.PlayerList;

        List<(string, int)> playersInfos = new List<(string, int)>();
        
        // Real players
        foreach (var p in players)
            playersInfos.Add((p.NickName, p.GetScore()));

        // Bots
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in allPlayers)
        {
            Bot bot = p.GetComponent<Bot>();
            if (bot)
                playersInfos.Add(("Bot", bot.score));
        }
            
        
        (string, int)[] infosArray = playersInfos.ToArray();

        Array.Sort(infosArray, (p1, p2) => p2.Item2.CompareTo(p1.Item2));
        string nicknameText = "";
        string scoreText = "";
        GetScoreBoardText(infosArray, ref nicknameText, ref scoreText);
        leaderboardText.text = nicknameText;
        leaderScoresText.text = scoreText;
    }
    
    
    private void Update()
    {
        if (leaderboardPanel.activeInHierarchy)
            UpdateLeaderboard();
        
        if (!beginned)
        {
            if (FindObjectsOfType<PhotonView>().Length - 1 >= PhotonNetwork.PlayerList.Length * 3) // - 1 for the crate
            {
                countdownTime = countdownDuration;
                beginTime = Time.realtimeSinceStartup;
                beginned = true;
            }
            else
                return;
        }

        if (countdownTime > 0f)
        {
            countdownTime = beginTime + countdownDuration - Time.realtimeSinceStartup;

            if (countdownTime <= 0f)
            {
                NewTextAnim(countdownText, "GO!");
                GameObject.Destroy(countdownText.gameObject, 0.5f);

                StartCoroutine(HideCenterPanelCoroutine());
                
                LevelManager.instance.gameStarted = true;

                beginTime = Time.realtimeSinceStartup;
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
            else
                countdownSlider.value += Time.deltaTime;

            return;
        }
        
        if (!endPanel.activeInHierarchy)
        {
            timeRemaining = beginTime + gameDuration - Time.realtimeSinceStartup;
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

    private IEnumerator HideCenterPanelCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        LeanTween.scale(centerPanel, Vector3.zero, 0.2f).setEaseInBack();
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

    private void GetScoreBoardText((string, int)[] infos, ref string nicknames, ref string scores)
    {
        for (int i = 0; i < infos.Length - 1; i++)
        {
            nicknames += infos[i].Item1 + "\n";
            scores += infos[i].Item2 + "\n";
        }

        nicknames += infos[infos.Length - 1].Item1;
        scores += infos[infos.Length - 1].Item2;
    }

    private void End()
    {
        // disable pause menu if enabled
        if (pausePanel.activeInHierarchy)
            Pause();

        LeanTween.scale(leaderboardPanel, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(() => leaderboardPanel.SetActive(false));
        
        NewTextAnim(secondsText, "0");

        
        Player[] players = PhotonNetwork.PlayerList;

        List<(string, int)> playersInfos = new List<(string, int)>();
        
        // Real players
        foreach (var p in players)
            playersInfos.Add((p.NickName, p.GetScore()));

        // Bots
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in allPlayers)
        {
            Bot bot = p.GetComponent<Bot>();
            if (bot)
            {
                playersInfos.Add(("Bot", bot.score));
            }
        }
        
        (string, int)[] infosArray = playersInfos.ToArray();

        Array.Sort(infosArray, (p1, p2) => p2.Item2.CompareTo(p1.Item2));
        string nicknameText = "";
        string scoreText = "";
        GetScoreBoardText(infosArray, ref nicknameText, ref scoreText);
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
        
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.LoadLevel(0);
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        chatView.GetComponent<Chat>().SetVoiceStatus();
    }
}
