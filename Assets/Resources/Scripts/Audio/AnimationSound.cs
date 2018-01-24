using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using System;

public class FootAttribute : PropertyAttribute { }

public class AnimationSound : MonoBehaviour
{
    [Serializable]
    public class Sound
    {
        [EventRef] public string Event = "";
        public AudioMaterial.eSurface SurfaceType = AudioMaterial.eSurface.WOOD;
    }

    [Serializable]
    public class SoundEvent
    {
        public string Name = "";
        public Sound[] Sounds;
    }

    public LayerMask m_Layer;
    public bool m_IgnoreFirstSound = true;
    public float m_DestroyTime = 10f;

    [Header("Feet")]
    [Foot] public Transform m_tFrontRight;
    [Foot] public Transform m_tFrontLeft;
    [Foot] public Transform m_tBackRight;
    [Foot] public Transform m_tBackLeft;

    [Space]
    public SoundEvent[] m_Events;


    private string m_NextFoot = "";

    public void SetNextFoot(string a_foot)
    {
        m_NextFoot = a_foot;
    }

    public void PlaySound(string a_sound)
    {
        if (m_IgnoreFirstSound)
        {
            m_IgnoreFirstSound = false;
            return;
        }

        if (m_Events.Length == 0)
            return;

        int i;
        for (i = 0; i < m_Events.Length; ++i)
        {
            if (string.Compare(m_Events[i].Name, a_sound, StringComparison.OrdinalIgnoreCase) == 0)
                break;
        }

        if (i >= m_Events.Length)
            return;

        GameObject obj = new GameObject();
        TimedDestroy timed = obj.AddComponent<TimedDestroy>();
        timed.DestroyTime = m_DestroyTime;

        StudioEventEmitter stud = obj.AddComponent<StudioEventEmitter>();
        stud.PlayEvent = EmitterGameEvent.Created;
        stud.StopEvent = EmitterGameEvent.Destroy;

        stud.Group = eAudioGroup.SFX;

        Transform point = transform;

        switch (m_NextFoot.ToLower())
        {
            case "frontright":
                {
                    point = m_tFrontRight;
                    break;
                }
            case "frontleft":
                {
                    point = m_tFrontLeft;
                    break;
                }
            case "backright":
                {
                    point = m_tBackRight;
                    break;
                }
            case "backleft":
                {
                    point = m_tBackLeft;
                    break;
                }
        }

        obj.transform.position = point.position;

        if (m_Events[i].Sounds.Length > 0)
        {
            List<string> materials = new List<string>();
            AudioMaterial.eSurface type = AudioMaterial.eSurface.WOOD;

            //Do a raycast check for surface
            RaycastHit[] hits = Physics.RaycastAll(point.position + (Vector3.up), Vector3.down, 2, m_Layer);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    AudioMaterial mat = hit.collider.GetComponent<AudioMaterial>();
                    if (mat == null)
                        continue;

                    if (!mat.m_IsSurface)
                        materials.Add(mat.m_Material.ToString());
                    else
                        type = mat.m_Surface;
                }
            }

            foreach (var sound in m_Events[i].Sounds)
            {
                if (sound.SurfaceType == type)
                {
                    stud.Event = sound.Event;
                    break;
                }
            }

            if (stud.Event == "" || stud.Event == null)
            {
                stud.Event = m_Events[i].Sounds[0].Event;
            }

            //Get the names of all of the enums
            string[] names = Enum.GetNames(typeof(AudioMaterial.eMaterial));
            stud.Params = new ParamRef[names.Length];

            //loop through the parameters and set their values
            for(int k = 0; k < names.Length; ++k)
            {
                stud.Params[k] = new ParamRef();
                stud.Params[k].Name = names[k];

                if (materials.Contains(names[k]))
                    stud.Params[k].Value = 1;
                else
                    stud.Params[k].Value = 0;
            }
        }
        else
        {
            Destroy(obj);
            return;
        }
    }
}
