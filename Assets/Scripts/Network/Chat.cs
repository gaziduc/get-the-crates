using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    private Text[] texts;
    private Dropdown levelDropdown;
    private Dropdown winConditionDropdown;
    
    private void Start()
    {
        GameObject chat = GameObject.FindWithTag("Chat");

        texts = new Text[chat.transform.childCount];

        for (int i = 0; i < texts.Length; i++)
            texts[i] = chat.transform.GetChild(i).GetComponent<Text>();

        levelDropdown = GameObject.FindWithTag("LevelDropdown").GetComponent<Dropdown>();
        winConditionDropdown = GameObject.FindWithTag("WinConditionDropdown").GetComponent<Dropdown>();
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
    }

    [PunRPC]
    public void SetLevelDropdownStateRPC(int value)
    {
        levelDropdown.value = value;
    }
    
    [PunRPC]
    public void SetWinConditionStateRPC(int value)
    {
        winConditionDropdown.value = value;
    }
}
