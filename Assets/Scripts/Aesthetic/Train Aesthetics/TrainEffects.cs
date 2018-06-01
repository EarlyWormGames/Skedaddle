using UnityEngine;
using System.Collections;

/// <summary>
/// base class for all train effects to derive from.
/// </summary>
public class TrainEffects : MonoBehaviour {

    ////////////////////////////////
    ///     Public Variables     ///
    ////////////////////////////////

    public static float m_fSShakeSpacing;
    public static float m_fSShakeDuration;
    public static float m_fSTimeOffset;
    public float m_fMicroOffset;
    public float m_fMicroDuration;
    public bool m_bDontDeactivate;

    ////////////////////////////////
    ///     Internal Variables   ///
    ////////////////////////////////

    ////////////////////////////////
    ///     Protected Variables  ///
    ////////////////////////////////
    protected bool m_bIsShaking;
    ////////////////////////////////
    ///     Private Variables    ///
    ////////////////////////////////

    private bool m_bStartTimer;
    private float m_fMasterTimer;
    

    void Start()
    {
        OnStart();
        m_fMasterTimer = m_fSTimeOffset;
    }


    void Update()
    {
        
        m_fMasterTimer -= Time.deltaTime;
        if(m_fMasterTimer < 0)
        {
            if (m_bStartTimer)
            {
                OnDeactivation();
                m_fMasterTimer = m_fSShakeSpacing;
                m_bStartTimer = false;
            }
            else
            {
                OnActivation();
                m_fMasterTimer = m_fSShakeDuration;
                m_bStartTimer = true;
            }

        }
        if (m_bIsShaking || m_bDontDeactivate)
        {
            Active();
        }
        else
        {
            Unactive();
        }
        OnUpdate();
    }

    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void Active() { }
    public virtual void Unactive() { }
    public virtual void OnActivation() { }
    public virtual void OnDeactivation() { }


}
