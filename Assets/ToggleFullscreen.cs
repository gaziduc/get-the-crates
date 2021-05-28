using UnityEngine;
using UnityEngine.UI;

public class ToggleFullscreen : MonoBehaviour
{
    private Toggle toggle;
    
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.isOn = PlayerPrefs.GetInt("Screenmanager Fullscreen mode", 1) == 1;
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = toggle.isOn;
    }
}
