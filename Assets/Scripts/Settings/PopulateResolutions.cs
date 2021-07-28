using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateResolutions : MonoBehaviour
{
    private Resolution[] resolutions;
    private Dropdown resolutionDropdown;
    
    void Start()
    {
        resolutionDropdown = GetComponent<Dropdown>();
        
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + " FPS";
            options.Add(option);
            
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen, resolutions[resolutionDropdown.value].refreshRate);
        PlayerPrefs.SetInt("Screenmanager Resolution Width", resolutions[resolutionDropdown.value].width);
        PlayerPrefs.SetInt("Screenmanager Resolution Height", resolutions[resolutionDropdown.value].height);
        PlayerPrefs.SetInt("Screenmanager Fullscreen mode", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt("Screenmanager Refresh Rate", resolutions[resolutionDropdown.value].refreshRate);
        PlayerPrefs.Save();
    }
}
