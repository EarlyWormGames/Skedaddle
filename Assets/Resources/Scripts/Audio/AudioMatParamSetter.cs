using UnityEngine;
using System;
using System.Collections.Generic;
using FMODUnity;

public class AudioMatParamSetter : MonoBehaviour
{
    public void CheckForMaterials()
    {
        StudioEventEmitter instance = GetComponent<StudioEventEmitter>();

        if (!instance.IsPlaying())
            return;

        List<string> extras = new List<string>();
        RaycastHit[] hitinfo = Physics.RaycastAll(transform.position + (Vector3.up * 2f), -Vector3.up, 3f, LayerMask.NameToLayer("AudioMat"));
        if (hitinfo.Length > 0)
        {
            for (int j = 0; j < hitinfo.Length; ++j)
            {
                AudioMaterial mat = hitinfo[j].collider.gameObject.GetComponent<AudioMaterial>();
                if (mat != null)
                {
                    if (!mat.m_IsSurface)
                        extras.Add(mat.m_Material.ToString());
                }
            }
        }

        string[] names = Enum.GetNames(typeof(AudioMaterial.eMaterial));
        foreach (string name in names)
        {
            if (extras.Contains(name, StringComparison.OrdinalIgnoreCase))
            {
                instance.SetParameter(name, 1);
            }
            else
            {
                instance.SetParameter(name, 0);
            }
        }
    }
}
