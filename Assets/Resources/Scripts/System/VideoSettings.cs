using UnityEngine;
using System.Collections;

public class VideoSettings : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    public void SetTextureQuality(int a_index)
    {
        QualitySettings.masterTextureLimit = 4 - a_index;        
    }
}
