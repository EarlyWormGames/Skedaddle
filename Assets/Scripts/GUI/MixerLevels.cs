using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Audio Mixer manager
/// mainly contains helper functions to manage the audio levels
/// </summary>
public class MixerLevels : MonoBehaviour {

    public AudioMixer masterMixer;
    public float CurrentMusicLvl;
    public float CurrentSFXLvl;

    public Toggle MusicMuteTgl;
    public Toggle SFXMuteTgl;

    // Use this for initialization
    void Start () {
	
	}
	
    
    public void SetSfxLvl(float a_sfxLvl)
    {
        masterMixer.SetFloat("sfxVolume", a_sfxLvl);
    }

    public void SetMusicLvl(float a_musicLvl)
    {
        masterMixer.SetFloat("MusicVolume", a_musicLvl);
    }

    public void MuteMusic()
    {
        if(MusicMuteTgl.isOn == true)
        {
            masterMixer.GetFloat("MusicVolume", out CurrentMusicLvl);
            masterMixer.SetFloat("MusicVolume", -80f);
            EWDebug.Log("Music Muted");
        }
        else
        {
            EWDebug.Log(CurrentMusicLvl);
            masterMixer.SetFloat("MusicVolume", CurrentMusicLvl);
            EWDebug.Log("Music Restored");
        }
        
    }

    public void MuteSFX()
    {
        if (SFXMuteTgl.isOn == true)
        {
            masterMixer.GetFloat("sfxVolume", out CurrentMusicLvl);
            masterMixer.SetFloat("sfxVolume", -80f);
            EWDebug.Log("SFX Muted");
        }
        else
        {
            EWDebug.Log(CurrentMusicLvl);
            masterMixer.SetFloat("sfxVolume", CurrentMusicLvl);
            EWDebug.Log("SFX Restored");
        }
    }

}
