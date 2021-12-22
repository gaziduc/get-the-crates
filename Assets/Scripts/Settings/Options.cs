using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Options : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private Text[] keyTexts;
    [SerializeField] private Text[] gamepadTexts;
    [SerializeField] private GameObject skinsPanel;

    public static Options instance;
    
    private bool getKey = false;
    private bool isController = false;
    private int controlNum;
    private string action = "";
    
    public void ToggleOptions()
    {
        if (optionsPanel.activeInHierarchy)
        {
            LeanTween.scale(optionsPanel, Vector3.zero, 0.2f).setEaseInBack().setOnComplete(DeactivateOptionsPanel);
        }
        else
        {
            optionsPanel.SetActive(true);
            optionsPanel.transform.localScale = Vector3.zero;
            LeanTween.scale(optionsPanel, Vector3.one, 0.2f).setEaseOutBack();
        }
    }

    void DeactivateOptionsPanel()
    {
        optionsPanel.SetActive(false);
    }


    public enum Controls
    {
        Shoot = 0,
        Jump = 1,
        Left = 2,
        Right = 3,
        NumControls = 4,
    }
    
    [HideInInspector] public string[] defaultControls = { "Space", "UpArrow", "LeftArrow", "RightArrow" };
    public string[] defaultControlsGamepad = { "rightTrigger", "buttonSouth", "leftStick/left", "leftStick/right" };

    public void ShowControlsPanel(int controlNum)
    {
        this.controlNum = controlNum;
        this.action = ((Controls) controlNum).ToString();
        
        controlsPanel.SetActive(true);
        controlsPanel.transform.GetChild(0).GetComponent<Text>().text = "Press new keyboard key for:";
        controlsPanel.transform.GetChild(1).GetComponent<Text>().text = this.action;
        controlsPanel.transform.GetChild(2).GetComponent<Text>().text = "(Escape to cancel)";
        isController = false;
        getKey = true;
    }

    public void ShowControlsPanelController(int controlNum)
    {
        this.controlNum = controlNum;
        this.action = ((Controls) controlNum).ToString();
        
        controlsPanel.SetActive(true);
        controlsPanel.transform.GetChild(0).GetComponent<Text>().text = "Press new gamepad button for:";
        controlsPanel.transform.GetChild(1).GetComponent<Text>().text = this.action;
        controlsPanel.transform.GetChild(2).GetComponent<Text>().text = "(Escape / Start to cancel)";
        isController = true;
        getKey = true;
    }

    private void Start()
    {
        instance = this;
        
        for (int i = 0; i < (int) Controls.NumControls; i++)
        {
            keyTexts[i].text = ((KeyCode) PlayerPrefs.GetInt(((Controls) i).ToString(), (int) Enum.Parse(typeof(KeyCode), defaultControls[i]))).ToString();
            gamepadTexts[i].text = PlayerPrefs.GetString(((Controls) i).ToString() + "Controller", defaultControlsGamepad[i]);
        }
    }

    private void Update()
    {
        if (getKey)
        {
            if (isController)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    getKey = false;
                    controlsPanel.SetActive(false);
                    return;
                }
                
                var gamepad = Gamepad.current;
                if (gamepad != null)
                {
                    if (gamepad.startButton.wasPressedThisFrame)
                    {
                        getKey = false;
                        controlsPanel.SetActive(false);
                        return;
                    }
                    
                    for (int i = 0; i < gamepad.allControls.Count; i++)
                    {
                        var c = gamepad.allControls[i];
                        if (c is ButtonControl && ((ButtonControl) c).wasPressedThisFrame)
                        {
                            string newGamepadKey = c.path;
                            newGamepadKey = newGamepadKey.Substring(1);
                            newGamepadKey = newGamepadKey.Substring(newGamepadKey.IndexOf('/') + 1);
                            
                            PlayerPrefs.SetString(action + "Controller", newGamepadKey);
                            PlayerPrefs.Save();

                            gamepadTexts[controlNum].text = newGamepadKey;

                            getKey = false;
                            controlsPanel.SetActive(false);
                            return;
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    getKey = false;
                    controlsPanel.SetActive(false);
                    return;
                }
                
                int[] values = (int[]) Enum.GetValues(typeof(KeyCode));
                

                foreach (int val in values)
                {
                    if (val >= (int) KeyCode.Mouse0) // Just keyboard, not mouse/gamepad
                        break;
                    
                    if (Input.GetKeyDown((KeyCode) val))
                    {
                        PlayerPrefs.SetInt(action, val);
                        PlayerPrefs.Save();
                        
                        keyTexts[controlNum].text = ((KeyCode) val).ToString();

                        getKey = false;
                        controlsPanel.SetActive(false);
                        return;
                    }
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
            PlayerPrefs.DeleteKey(tempAction + "Controller");
            PlayerPrefs.Save();

            keyTexts[i].text = defaultControls[i];
            gamepadTexts[i].text = defaultControlsGamepad[i];
        }
    }

    public void SetSkin(int skinNum)
    {
        PlayerPrefs.SetInt("SkinNum", skinNum);
        PlayerPrefs.Save();

        Image image = skinsPanel.transform.GetChild(skinNum).GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.35f);

        for (int i = 0; i < 4; i++)
        {
            if (i == skinNum)
                continue;
            
            Image imageToDisable = skinsPanel.transform.GetChild(i).GetComponent<Image>();
            imageToDisable.color = new Color(imageToDisable.color.r, imageToDisable.color.g, imageToDisable.color.b, 0f);
        }
    }
    
}
