using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    
    // Start is called before the first frame update
    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        volumeSlider.value = AudioListener.volume;
    }

    public void SetMasterVolume()
    {
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
        PlayerPrefs.Save();
    }
}
