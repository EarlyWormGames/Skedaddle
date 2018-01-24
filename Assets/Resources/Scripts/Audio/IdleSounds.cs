using UnityEngine;
using System;
using FMODUnity;

public class IdleSounds : MonoBehaviour
{
    [Serializable]
    public class SoundEvent
    {
        public string Name = "";
        public StudioEventEmitter Sound;
    }

    public SoundEvent[] m_Events;

    public void PlaySound(string a_name)
    {
        foreach (var sound in m_Events)
        {
            if (a_name.ToLower() == sound.Name.ToLower())
            {
                if (!sound.Sound.IsPlaying())
                {
                    sound.Sound.Play();
                }
            }
            else
            {
                if (sound.Sound.IsPlaying())
                {
                    sound.Sound.TriggerCue();
                }
            }
        }
    }
}