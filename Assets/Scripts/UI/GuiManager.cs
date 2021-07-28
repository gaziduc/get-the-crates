using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text secondsText;
    [SerializeField] private Text tenthText;
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
    private Text[] infoTexts;
    
    private float timeRemaining = 90f;
    private float countdownTime = 3f;

    private bool beginned = false;
    private bool ended = false;

    void Start()
    {
        backToMenuButton.interactable = PhotonNetwork.IsMasterClient;
        backToMenuButtonEnd.interactable = PhotonNetwork.IsMasterClient;

        infoTexts = new Text[infoPanel.transform.childCount];
        for (int i = 0; i < infoTexts.Length; i++)
        {
            infoTexts[i] = infoPanel.transform.GetChild(i).GetComponent<Text>();
            infoTexts[i].text = "";
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        for (int i = 0; i < infoTexts.Length - 1; i++)
            infoTexts[i].text = infoTexts[i + 1].text;

        infoTexts[infoTexts.Length - 1].text = "<color=cyan>" + otherPlayer.NickName + " <color=red>left the room.</color></color>";
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        bool amIMaster = newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
        
        backToMenuButton.interactable = amIMaster;
        backToMenuButtonEnd.interactable = amIMaster;
    }


    private void SetTimeText()
    {
        secondsText.text = ((int) timeRemaining).ToString();
        tenthText.text = "." + (int) (timeRemaining % 1 * 10);
    }
    
    private void Update()
    {
        if (!beginned)
        {
            if (FindObjectsOfType<PhotonView>().Length - 1 == PhotonNetwork.PlayerList.Length * 2) // - 1 for the crate
                beginned = true;
            else
                return;
        }

        if (countdownTime > 0f)
        {
            string lastText = countdownText.text;
            countdownTime -= Time.deltaTime;

            if (countdownTime <= 0f)
            {
                countdownText.text = "GO!";
                GameObject.Destroy(countdownText.gameObject, 0.5f);

                LevelManager.instance.gameStarted = true;
                
                countdownGoSound.Play();
                GameObject.Instantiate(countdownEffect, Vector3.zero, Quaternion.identity);
                return;
            }
            
            countdownText.text = ((int) countdownTime + 1).ToString();

            if (!lastText.Equals(countdownText.text))
            {
                countdownSound.Play();
                GameObject.Instantiate(countdownEffect, Vector3.zero, Quaternion.identity);
            }
            
            return;
        }
        
        if (!endPanel.activeInHierarchy)
        {
            timeRemaining -= Time.deltaTime;
            SetTimeText();

            if (!ended && timeRemaining <= 0f)
            {
                ended = true;
                End();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(!pausePanel.activeInHierarchy);
    }

    private void End()
    {
        Player[] players = PhotonNetwork.PlayerList;
        Array.Sort(players, (p1, p2) => p2.GetScore().CompareTo(p1.GetScore()));
        
        string nicknameText = "";
        string scoreText = "";
    
        for (int i = 0; i < players.Length - 1; i++)
        {
            nicknameText += players[i].NickName + "\n";
            scoreText += players[i].GetScore() + "\n";
        }

        nicknameText += players[players.Length - 1].NickName;
        scoreText += players[players.Length - 1].GetScore();

        scoreboardText.text = nicknameText;
        scoresText.text = scoreText;
        
        end.Play();
        endPanel.SetActive(true);
    }

    public void GoBackToMenu()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.LoadLevel(0);
        }
    }
}
