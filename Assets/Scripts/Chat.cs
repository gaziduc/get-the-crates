using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    private Text[] texts;
    
    private void Start()
    {
        GameObject chat = GameObject.FindWithTag("Chat");

        texts = new Text[chat.transform.childCount];

        for (int i = 0; i < texts.Length; i++)
            texts[i] = chat.transform.GetChild(i).GetComponent<Text>();
    }

    public void ClearMessages()
    {
        foreach (Text t in texts)
        {
            t.text = "";
        }
    }


    [PunRPC]
    void SendMessageRPC(string msg)
    {
        for (int i = 0; i < texts.Length - 1; i++)
            texts[i].text = texts[i + 1].text;

        texts[texts.Length - 1].text = msg;
    }
}
