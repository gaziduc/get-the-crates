using System;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    private Text[] texts;
    private Dropdown levelDropdown;
    private Dropdown winConditionDropdown; 
    private AudioSource chatSound;
    private PhotonView view;
    private PhotonVoiceView voiceView;
    private Image[] voiceImages;
    

    private void Start()
    {
        view = GetComponent<PhotonView>();
        voiceView = GetComponent<PhotonVoiceView>();
        
        chatSound = GameObject.FindWithTag("AudioSources").transform.GetChild(1).GetChild(4).GetComponent<AudioSource>();

        GameObject chat = GameObject.FindWithTag("Chat");
    
        texts = new Text[chat.transform.childCount];

        for (int i = 0; i < texts.Length; i++)
            texts[i] = chat.transform.GetChild(i).GetComponent<Text>();

        levelDropdown = GameObject.FindWithTag("LevelDropdown").GetComponent<Dropdown>();
        winConditionDropdown = GameObject.FindWithTag("WinConditionDropdown").GetComponent<Dropdown>();
        
        GameObject playerList = GameObject.FindWithTag("PlayerList");

        voiceImages = new Image[4];

        for (int i = 0; i < playerList.transform.childCount; i++)
        {
            voiceImages[i] = playerList.transform.GetChild(i).GetChild(0).GetComponent<Image>();
            voiceImages[i].enabled = false;
        }
    }
    

    public void ClearMessages()
    {
        foreach (Text t in texts)
        {
            t.text = "";
        }
    }


    [PunRPC]
    public void SendMessageRPC(string msg)
    {
        for (int i = 0; i < texts.Length - 1; i++)
            texts[i].text = texts[i + 1].text;

        texts[texts.Length - 1].text = msg;
        
        chatSound.Play();
    }

    [PunRPC]
    public void SetLevelDropdownStateRPC(int value)
    {
        levelDropdown.value = value;
        
        chatSound.Play();
    }
    
    [PunRPC]
    public void SetWinConditionStateRPC(int value)
    {
        winConditionDropdown.value = value;
        
        chatSound.Play();
    }

    [PunRPC]
    public void SetVoiceStatus(bool isSpeaking, int actorNum)
    {
        Player player = PhotonNetwork.LocalPlayer.Get(actorNum);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].ActorNumber == player.ActorNumber)
            {
                voiceImages[i].enabled = isSpeaking;
                break;
            }
        }
    }
}
