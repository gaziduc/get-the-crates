using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    [SerializeField] private Text[] Nicknames;
    [SerializeField] private Slider[] Healths;
    public Gradient gradient;
    [SerializeField] private Image[] fill;
    private int[] scores;
    [SerializeField] private Text[] scoresTexts;
    [SerializeField] private Text secondsText;
    [SerializeField] private Text tenthText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Text winnerText;
    [SerializeField] private Text looserText;
    public GameObject pausePanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMenuButtonEnd;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private AudioSource end;
    
    private float timeRemaining = 60f;
    
    void Start()
    {
        backToMenuButton.interactable = PhotonNetwork.IsMasterClient;
        restartButton.interactable = PhotonNetwork.IsMasterClient;
        backToMenuButtonEnd.interactable = PhotonNetwork.IsMasterClient;

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
        secondsText.text = ((int) timeRemaining).ToString();
        tenthText.text = "." + (int) (timeRemaining % 1 * 10);
    }


    private void Update()
    {
        if (!endPanel.activeInHierarchy)
        {
            if (!pausePanel.activeInHierarchy)
            {
                timeRemaining -= Time.deltaTime;
                SetTimeText();

                if (timeRemaining <= 0f)
                    ShowEnd();
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

    public void ShowEnd()
    {
        end.Play();

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView view = GameObject.FindWithTag("Player").GetComponent<PhotonView>();
            view.RPC("EndRPC", RpcTarget.Others);
        }
        
        string winnerNickname = "";
        string looserNickname = "";
        
        if (scores[0] > scores[1])
        {
            winnerNickname = PhotonNetwork.PlayerList[0].NickName + " (" + scores[0] + ")";

            if (PhotonNetwork.PlayerList.Length > 1)
                looserNickname = PhotonNetwork.PlayerList[1].NickName + " (" + scores[1] + ")";
            else
                looserNickname = "No looser";
        }
        else
        {
            winnerNickname = PhotonNetwork.PlayerList[1].NickName + " (" + scores[1] + ")";
            looserNickname = PhotonNetwork.PlayerList[0].NickName + " (" + scores[0] + ")";
        }
        
        winnerText.text = "Winner: " + winnerNickname;
        looserText.text = "Looser: " + looserNickname;
        
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
