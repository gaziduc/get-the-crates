using System;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private Text[] keyTexts;

    public static Options instance;
    
    private bool getKey = false;
    private int controlNum;
    private string action = "";
    
    public void ToggleOptions()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
    }


    public enum Controls
    {
        Shoot = 0,
        Jump = 1,
        Left = 2,
        Right = 3,
        NumControls = 4,
    }
    
    public string[] defaultControls = { "Space", "UpArrow", "LeftArrow", "RightArrow" };

    public void ShowControlsPanel(int controlNum)
    {
        this.controlNum = controlNum;
        this.action = ((Controls) controlNum).ToString();
        
        controlsPanel.SetActive(true);
        controlsPanel.transform.GetChild(1).GetComponent<Text>().text = this.action;
        getKey = true;
    }

    private void Start()
    {
        instance = this;
        
        for (int i = 0; i < (int) Controls.NumControls; i++)
        {
            keyTexts[i].text = ((KeyCode) PlayerPrefs.GetInt(((Controls) i).ToString(), (int) Enum.Parse(typeof(KeyCode), defaultControls[i]))).ToString();
        }
    }

    private void Update()
    {
        if (getKey)
        {
            int[] values = (int[]) Enum.GetValues(typeof(KeyCode));
            
            for (int i = 0; i < values.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    getKey = false;
                    controlsPanel.SetActive(false);
                    return;
                }
                
                if (Input.GetKeyDown((KeyCode) values[i]))
                {
                    PlayerPrefs.SetInt(action, values[i]);
                    PlayerPrefs.Save();

                    keyTexts[this.controlNum].text = ((KeyCode) values[i]).ToString();

                    getKey = false;
                    controlsPanel.SetActive(false);
                    return;
                }
            }
        }
    }

    public void ResetCommands()
    {
        for (int i = 0; i < (int) Controls.NumControls; i++)
        {
            string tempAction = ((Controls) i).ToString();
            PlayerPrefs.DeleteKey(tempAction);
            PlayerPrefs.Save();

            keyTexts[i].text = defaultControls[i];
        }
    }
}
