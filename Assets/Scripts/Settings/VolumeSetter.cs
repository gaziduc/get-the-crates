using UnityEngine;

public class VolumeSetter : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource[] sfx;
    
    // Start is called before the first frame update
    void Start()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.3f);
        float sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.3f);
        
        music.volume = musicVolume;
        foreach (var sound in sfx)
            sound.volume = sfxVolume;
    }
}
