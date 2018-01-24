using UnityEngine;
using System.Collections;

public class MasterTimer : TrainEffects {

    public float m_fMasterSpacing;
    public float m_fMasterDuration;
    public float m_fMasterOffset;

    void Awake()
    {
        m_fSShakeSpacing = m_fMasterSpacing;
        m_fSShakeDuration = m_fMasterDuration;
        m_fSTimeOffset = m_fMasterOffset;
    }
}
