using UnityEngine;
using System.Collections;
using FMODUnity;

[System.Serializable]
public class AudioSelectObj
{
    public StudioEventEmitter sound;
    public AudioMaterial.eSurface surface;
}

public class AudioSelector : MonoBehaviour
{
    public LayerMask layer;
    public AudioSelectObj[] sounds;

    public void PlayAll()
    {
        foreach (AudioSelectObj obj in sounds)
        {
            obj.sound.Stop();
            obj.sound.Play();
        }
    }

    public void SurfaceTrigger()
    {
        AudioMaterial.eSurface type = AudioMaterial.eSurface.WOOD;
        RaycastHit[] hitinfo = Physics.RaycastAll(transform.position, -Vector3.up, 2f, layer);
        if (hitinfo.Length > 0)
        {
            for (int j = 0; j < hitinfo.Length; ++j)
            {
                AudioMaterial mat = hitinfo[j].collider.gameObject.GetComponent<AudioMaterial>();
                if (mat != null)
                {
                    if (mat.m_IsSurface)
                    {
                        type = mat.m_Surface;
                        break;
                    }
                }
            }
        }

        foreach (AudioSelectObj obj in sounds)
        {
            if (obj.surface == type)
            {
                obj.sound.TriggerCue();
                obj.sound.GetComponent<AudioMatParamSetter>().CheckForMaterials();
            }
            else
            {
                obj.sound.Stop();                
            }
        }
    }

    public void StopAll()
    {
        foreach (AudioSelectObj obj in sounds)
        {
            obj.sound.Stop();
        }
    }
}
