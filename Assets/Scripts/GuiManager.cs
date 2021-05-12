using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    [SerializeField] private Text[] Nicknames;
    [SerializeField] private Text[] Healths;

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
    
    public void SetHealthText(int health, int playerNum)
    {
        Healths[playerNum].text = "Health: " + health;
    }
    
    
    
}
