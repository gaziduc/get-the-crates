using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Chat : MonoBehaviour
{
    private PhotonView view;
    private bool changedScene = false;
    private bool voiceSpeaking = false;
    public bool typing = false;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        
        SetPlayerCustomProps();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneLoaded;
        
        ResetVoiceStatusForRemainingPlayers();
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (view.IsMine)
            changedScene = true;
    }

    private IEnumerator WaitForPlayerList()
    {
        GameObject playerList = null;
        
        do
        { 
            playerList = GameObject.FindWithTag("PlayerList");
            yield return null;
        } while (playerList == null);

        Text typingText = null;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            typingText = GameObject.FindWithTag("IsTypingText").GetComponent<Text>();
            typingText.text = "";
        }
        

        bool firstTyping = true;
        bool severalTyping = false;
        
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            bool isVoiceEnabled = false;
            
            if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("voice"))
                isVoiceEnabled = (bool) PhotonNetwork.PlayerList[i].CustomProperties["voice"];

            playerList.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = isVoiceEnabled;
            
            if (SceneManager.GetActiveScene().buildIndex == 0)
                playerList.transform.GetChild(i).GetChild(2).GetComponent<Image>().enabled = !isVoiceEnabled;
            
            playerList.transform.GetChild(i).GetChild(GetSpeakerImageChildIndex()).GetComponent<Image>().enabled = false;
            
            SetNickname(i, isVoiceEnabled, PhotonNetwork.PlayerList[i]);
            
            // Typing status
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                bool isTyping = false;
            
                if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("typing"))
                    isTyping = (bool) PhotonNetwork.PlayerList[i].CustomProperties["typing"];

                if (isTyping)
                {
                    if (!firstTyping)
                    {
                        severalTyping = true;
                        typingText.text += ", ";
                    }
                        
                
                    firstTyping = false;
                    typingText.text += "<color=cyan>" + PhotonNetwork.PlayerList[i].NickName + "</color>";
                }
            }
        }

        if (!firstTyping && !severalTyping)
            typingText.text += " is typing...";
        else if (severalTyping)
            typingText.text += " are typing...";

        for (int i = PhotonNetwork.PlayerList.Length; i < playerList.transform.childCount; i++)
        {
            playerList.transform.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
            
            if (SceneManager.GetActiveScene().buildIndex == 0)
                playerList.transform.GetChild(i).GetChild(2).GetComponent<Image>().enabled = false;
            
            playerList.transform.GetChild(i).GetChild(GetSpeakerImageChildIndex()).GetComponent<Image>().enabled = false;
            
            SetNickname(i, false, null);
        }
        
        
    }
    

    public void ClearMessages()
    {
        GameObject chat = GameObject.FindWithTag("Chat");
        
        for (int i = 0; i < chat.transform.childCount; i++)
            chat.transform.GetChild(i).GetComponent<Text>().text = "";
    }


    [PunRPC]
    public void SendMessageRPC(string msg)
    {
        GameObject chat = GameObject.FindWithTag("Chat");
        
        for (int i = 0; i < chat.transform.childCount - 1; i++)
            chat.transform.GetChild(i).GetComponent<Text>().text = chat.transform.GetChild(i + 1).GetComponent<Text>().text;

        chat.transform.GetChild(chat.transform.childCount - 1).GetComponent<Text>().text = msg;
        
        GameObject.FindWithTag("AudioSources").transform.GetChild(1).GetChild(4).GetComponent<AudioSource>().Play();
    }

    [PunRPC]
    public void SetNumBotsStateRPC(int value)
    {
        GameObject.FindWithTag("NumBotsDropdown").GetComponent<Dropdown>().value = value;
        GameObject.FindWithTag("AudioSources").transform.GetChild(1).GetChild(4).GetComponent<AudioSource>().Play();
    }
    
    [PunRPC]
    public void SetLevelDropdownStateRPC(int value)
    {
        GameObject.FindWithTag("LevelDropdown").GetComponent<Dropdown>().value = value;
        GameObject.FindWithTag("AudioSources").transform.GetChild(1).GetChild(4).GetComponent<AudioSource>().Play();
    }
    
    [PunRPC]
    public void SetWinConditionStateRPC(int value)
    {
        GameObject.FindWithTag("WinConditionDropdown").GetComponent<Dropdown>().value = value;
        GameObject.FindWithTag("AudioSources").transform.GetChild(1).GetChild(4).GetComponent<AudioSource>().Play();
    }

    [PunRPC]
    void StartTransitionRPC()
    {
        LeanTween.scale(GameObject.FindWithTag("TransitionPanel"), new Vector3(2.2f, 2.2f,2.2f), 0.4f).setEaseInCubic();
    }

    public void SetVoiceAndTypingStatus()
    {
        StartCoroutine(WaitForPlayerList());
    }
    

    private void SetNickname(int i, bool voiceEnabled, Player player)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            GameObject playerList = GameObject.FindWithTag("PlayerList");
            
            Text text = playerList.transform.GetChild(i).GetChild(1).GetComponent<Text>();
            text.enabled = voiceEnabled;
            if (text.enabled)
                text.text = player.NickName;
        }
    }
    

    public void ResetVoiceStatusForRemainingPlayers()
    {
        StartCoroutine(ResetCoroutine());
    }

    private IEnumerator ResetCoroutine()
    {
        GameObject playerList = null;
        
        do
        { 
            playerList = GameObject.FindWithTag("PlayerList");
            yield return null;
        } while (playerList == null);

        for (int i = PhotonNetwork.PlayerList.Length; i < playerList.transform.childCount; i++)
        {
            Transform currChild = playerList.transform.GetChild(i);
            
            currChild.GetChild(0).GetComponent<Image>().enabled = false;
            
            if (SceneManager.GetActiveScene().buildIndex == 0)
                currChild.GetChild(2).GetComponent<Image>().enabled = false;

            currChild.GetChild(GetSpeakerImageChildIndex()).GetComponent<Image>().enabled = false;
            
            SetNickname(i, false, null);
        }
    }


    private void Update()
    {
        if (view.IsMine)
        {
            if (changedScene)
            {
                changedScene = false;
                StartCoroutine(WaitForPlayerList());
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                    ToggleVoiceChat();

                GameObject playerList = GameObject.FindWithTag("PlayerList");
                if (playerList == null)
                    return;
                
                PhotonVoiceView[] voiceViews = GameObject.FindObjectsOfType<PhotonVoiceView>();

                foreach (var voiceView in voiceViews)
                {
                    PhotonView photonView = voiceView.GetComponent<PhotonView>();

                    for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                    {
                        if (PhotonNetwork.PlayerList[i].ActorNumber == photonView.OwnerActorNr)
                        {
                            if (photonView.IsMine)
                                playerList.transform.GetChild(i).GetChild(GetSpeakerImageChildIndex()).GetComponent<Image>().enabled = voiceView.IsRecording;
                            else
                                playerList.transform.GetChild(i).GetChild(GetSpeakerImageChildIndex()).GetComponent<Image>().enabled = voiceView.IsSpeaking;
                        }
                    }
                }
            }
        }
    }

    private int GetSpeakerImageChildIndex()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return 3;

        return 2;
    }
    
    public void ToggleVoiceChat()
    {
        #if UNITY_WEBGL
            if (SceneManager.GetActiveScene().buildIndex == 0)
                SendMessageRPC("<color=lime>Please download the game to get voice chat.</color>");
            else
                GameObject.FindWithTag("PlayerManager").GetComponent<GuiManager>().AddMessage("<color=lime>Please download the game to get voice chat.</color>");
        #else
            Recorder recorder = GetComponent<Recorder>();

            if (recorder.enabled)
            {
                recorder.TransmitEnabled = false;
                voiceSpeaking = false;
                recorder.enabled = false;
            }
            else
            {
                recorder.TransmitEnabled = true;
                voiceSpeaking = true;
                recorder.enabled = true;
            }

            SetPlayerCustomProps(); 
        #endif
    }

    public void SetPlayerCustomProps()
    {
        if (view.IsMine)
        {
            // Set custom player properties
            Hashtable props = new Hashtable();
            props.Add("voice", voiceSpeaking);
            props.Add("typing", typing);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
    }
}
