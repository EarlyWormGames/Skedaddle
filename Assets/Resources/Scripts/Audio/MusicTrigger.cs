using UnityEngine;
using System.Collections;

public class MusicTrigger : MonoBehaviour
{
    public enum eTriggerType
    {
        TRACK,
        PARAMETER,
    }

    public string m_TrackName = "";
    public string m_ParameterName = "";
    public float m_ParameterValue = 1f;
    public eTriggerType m_Type = eTriggerType.TRACK;
    public MusicHandler.TrackType m_TrackType = MusicHandler.TrackType.MUSIC;


    private bool m_Triggered = false;

    // Use this for initialization
    void Start()
    {
        if (m_Type == eTriggerType.TRACK)
            MusicHandler.PlayTrack(m_TrackName, m_TrackType);
    }

    void OnTriggerEnter(Collider a_col)
    {
        if (m_Triggered)
            return;

        if (m_Type != eTriggerType.TRACK)
        {
            MusicHandler.SetParameter(m_ParameterName, m_ParameterValue);
        }
    }

    void OnDestroy()
    {
        if (m_Type != eTriggerType.TRACK)
            MusicHandler.SetParameter(m_ParameterName, 0);
        else
            MusicHandler.StopTrack(m_TrackName, m_TrackType);
    }
}