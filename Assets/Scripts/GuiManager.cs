using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    [SerializeField] private Text secondsText;
    [SerializeField] private Text tenthText;
    [SerializeField] private GameObject endPanel;
    public GameObject pausePanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMenuButtonEnd;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private AudioSource end;
    [SerializeField] private Text countdownText;
    [SerializeField] private AudioSource countdownSound;
    [SerializeField] private AudioSource countdownGoSound;
    [SerializeField] private GameObject countdownEffect;
    [SerializeField] private Text scoreboardText;
    [SerializeField] private Text scoresText;
    
    private float timeRemaining = 90f;
    private float countdownTime = 3f;

    private bool beginned = false;
    private bool ended = false;

    void Start()
    {
        backToMenuButton.interactable = PhotonNetwork.IsMasterClient;
        restartButton.interactable = PhotonNetwork.IsMasterClient;
        backToMenuButtonEnd.interactable = PhotonNetwork.IsMasterClient;
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
            if (FindObjectsOfType<PhotonView>().Length - 2 == PhotonNetwork.PlayerList.Length) // - 2 for the crates
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
                GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().canMove = true;
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
            if (!pausePanel.activeInHierarchy)
            {
                timeRemaining -= Time.deltaTime;
                SetTimeText();

                if (!ended && timeRemaining <= 0f)
                {
                    ended = true;
                    End();
                }
                    
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        PhotonView view = GameObject.FindWithTag("Player").GetComponent<PhotonView>();
        view.RPC("PauseRPC", RpcTarget.All);
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
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.LoadLevel(0);
        }
    }
}
