using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider voiceVolumeSlider;

    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource[] sfx;
    
    [SerializeField] private NetworkManager net;

    // Start is called before the first frame update
    void Start()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.4f);
        voiceVolumeSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 0.85f);
        
        
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        music.volume = musicVolumeSlider.value;
        foreach (var sound in sfx)
            sound.volume = sfxVolumeSlider.value;
        
        net.ApplyVoiceVolume(voiceVolumeSlider.value);
    }

    public void SetMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.Save();
        
        ApplyVolume();
    }

    public void SetSfxVolume()
    {
        PlayerPrefs.SetFloat("SfxVolume", sfxVolumeSlider.value);
        PlayerPrefs.Save();
        
        ApplyVolume();
    }
    
    public void SetVoiceVolume()
    {
        PlayerPrefs.SetFloat("VoiceVolume", voiceVolumeSlider.value);
        PlayerPrefs.Save();
        
        ApplyVolume();
    }
}
