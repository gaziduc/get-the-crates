using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    [SerializeField] private Text[] Nicknames;
    [SerializeField] private Slider[] Healths;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image[] fill;
    private int[] scores;
    [SerializeField] private Text[] scoresTexts;
    [SerializeField] private Text timeText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Text winnerText;
    [SerializeField] private Text looserText;
    public GameObject pausePanel;

    private float timeRemaining = 60;
    
    void Start()
    {
        SetTimeText();
        scores = new int[] { 0, 0 };
    }

    public void SetNicknameText(string nickname, int playerNum)
    {
        Nicknames[playerNum].text = "<color=" + GetPlayerColor(playerNum) + ">P" + (playerNum + 1) + ": " + nickname + "</color>";
    }

    public string GetPlayerColor(int playerNum)
    {
        if (playerNum == 0)
            return "cyan";
        
        return "red";
    }

    public void SetMaxHealth(int health, int playerNum)
    {
        Healths[playerNum].maxValue = health;
        SetHealthBar(health, playerNum);
    }
    
    public void SetHealthBar(int health, int playerNum)
    {
        Healths[playerNum].value = health;
        fill[playerNum].color = gradient.Evaluate(Healths[playerNum].normalizedValue);
    }

    public void SetScore(int score, int playerNum)
    {
        scores[playerNum] = score;
        scoresTexts[playerNum].text = "Score: " + score;
    }


    private void SetTimeText()
    {
        timeText.text = ((int) timeRemaining).ToString();
    }


    private void Update()
    {
        if (!endPanel.activeInHierarchy)
        {
            timeRemaining -= Time.deltaTime;
            SetTimeText();

            if (timeRemaining <= 0f)
                ShowEnd();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PhotonView view = GameObject.FindWithTag("Player").GetComponent<PhotonView>();
                view.RPC("PauseRPC", RpcTarget.All);
            }
        }
        
    }

    public void ShowEnd()
    {
        endPanel.SetActive(true);
        
        if (scores[0] > scores[1])
        {
            winnerText.text = "Winner: " + PhotonNetwork.PlayerList[0].NickName + " (" + scores[0] + ")";
            looserText.text = "Looser: " + PhotonNetwork.PlayerList[1].NickName + " (" + scores[1] + ")";
        }
        else 
        {
            winnerText.text = "Winner: " + PhotonNetwork.PlayerList[1].NickName + " (" + scores[1] + ")";
            looserText.text = "Looser: " + PhotonNetwork.PlayerList[0].NickName + " (" + scores[0] + ")";
        }
    }
}
