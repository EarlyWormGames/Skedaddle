using UnityEngine;
using System.Collections;

public class AreaMusic : MonoBehaviour
{
    public float m_fLerpSpeed = 1f;

    internal bool isPlaying
    {
        get
        {
            return m_bPlaying;
        }
    }

    private float m_fCurrentVal = 0f;
    private int m_iTargetIndex = 0;
    private bool m_bPlaying = false;

    // Update is called once per frame
    void Update()
    {
    }
}
