using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle muteVoiceChat;

    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource[] sfx;
    
    [SerializeField] private NetworkManager net;

    // Start is called before the first frame update
    void Start()
    {
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.4f);
        muteVoiceChat.isOn = PlayerPrefs.GetInt("MuteVoiceChat", 0) == 1;
        
        
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        music.volume = musicVolumeSlider.value;
        foreach (var sound in sfx)
            sound.volume = sfxVolumeSlider.value;
        
        net.ApplyVoiceVolume(muteVoiceChat.isOn);
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
        
        sfx[4].Play();
    }
    
    public void SetVoiceVolume()
    {
        PlayerPrefs.SetInt("MuteVoiceChat", muteVoiceChat.isOn ? 1 : 0);
        PlayerPrefs.Save();
        
        ApplyVolume();
    }
}
