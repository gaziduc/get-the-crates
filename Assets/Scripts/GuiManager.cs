using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    [SerializeField] private Text[] Nicknames;
    [SerializeField] private Slider[] Healths;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image[] fill;

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
}
