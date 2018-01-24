using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MovingObject : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public Transform    m_tEndPoint;
    public Transform    m_tRopes;
    public float        m_fEndScale;
    public float        m_fSlideTime = 1f;
    public bool         m_bRotate;
    public bool         m_bDontDeactivate;

    public Transform    m_tMovePoint;
    public Transform    m_tLookAt;
    public bool         m_bLook = true;
    public ActionObject m_oActivate;
    public float        m_fWaitTimer = 1f;
    public float        m_fLookTime = 1f;

    public Animator m_aAnimator;

    public AnimationCurve m_aCurve;
    public AnimationCurve m_aReverseCurve;
    public bool m_bUseReverse = false;

    public BoxCollider m_BoxCaster;
    public LayerMask m_Layer;

    public UnityEvent m_OnMoveEnd;
    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 
    private bool m_bDown = false;
    private bool m_bRequiredState = false;
    private Vector3 m_v3StartPos;
    private Quaternion m_qStartRot;
    private bool m_bMove = false;
    private float m_fTimer;
    private float m_fAnimTimer;

    private bool m_bStartMove = false;

    //Inherited functions

    protected override void OnStart()
    {
        m_v3StartPos = transform.position;
        m_qStartRot = transform.rotation;
    }

    protected override void OnUpdate()
    {
        if(m_aAnimator != null)
        {
            m_aAnimator.SetFloat("Fold", m_fAnimTimer);
        }
        if (m_bMove)
        {
            if (m_bStartMove)
            {
                m_bStartMove = false;
            }

            if (m_BoxCaster != null)
            {
                if(!Physics.CheckBox(m_BoxCaster.transform.TransformPoint(m_BoxCaster.center), m_BoxCaster.size / 2f, m_BoxCaster.transform.rotation, m_Layer.value))
                    m_fTimer += Time.deltaTime;
            }
            else
                m_fTimer += Time.deltaTime;

            m_fAnimTimer = m_fTimer;
            Vector3 start = m_bDown ? m_v3StartPos : m_tEndPoint.position;
            Vector3 end = m_bDown ? m_tEndPoint.position : m_v3StartPos;
            Quaternion rotStart = m_bDown ? m_qStartRot : m_tEndPoint.rotation;
            Quaternion rotEnd = m_bDown ? m_tEndPoint.rotation : m_qStartRot;
            float t = Mathf.Min(1, m_fTimer / m_fSlideTime);
            t = m_bUseReverse && !m_bDown? m_aReverseCurve.Evaluate(t) : m_aCurve.Evaluate(t);
            transform.position = Vector3.Lerp(start, end, t);
            if(m_tRopes != null)
            {
                m_tRopes.localScale = Vector3.Lerp(new Vector3(m_tRopes.localScale.x, 1, m_tRopes.localScale.z), new Vector3(m_tRopes.localScale.x, m_fEndScale, m_tRopes.localScale.z), 1 - t);
            }
            if (m_bRotate)
            {
                transform.rotation = Quaternion.Lerp(rotStart, rotEnd, t);
            }

            if (m_fTimer >= m_fSlideTime)
            {
                OnSlideEnd();
            }
        }
    }

    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        if (m_bMove)
            return;

        if (transform.position == m_v3StartPos)
        {
            m_bRequiredState = true;
            m_bDown = true;
        }
        else
        {
            m_bRequiredState = false;
            m_bDown = false;
        }

        PlaySound(m_bDown ? SOUND_EVENT.DOOR_OPEN : SOUND_EVENT.DOOR_CLOSE);
        m_bMove = true;
        m_fTimer = 0f;
    }

    public override void DoActionOn()
    {
        if (m_bMove)
            return;
        m_bRequiredState = true;
        PlaySound(SOUND_EVENT.DOOR_OPEN);
        m_bDown = true;
        m_bMove = true;
        m_fTimer = 0f;
    }

    public override void DoActionOff()
    {
        if (m_bMove)
            return;
        if (m_bDontDeactivate)
            return;
        m_bRequiredState = false;
        PlaySound(SOUND_EVENT.DOOR_CLOSE);
        m_bDown = false;
        m_bMove = true;
        m_fTimer = 0f;
    }

    void OnSlideEnd()
    {
        m_bStartMove = true;

        m_fTimer = 0f;
        m_bMove = false;

        if (m_bRequiredState != m_bDown)
        {
            m_bDown = m_bRequiredState;
            m_bMove = true;
            m_fTimer = 0f;
            PlaySound(m_bDown ? SOUND_EVENT.DOOR_OPEN : SOUND_EVENT.DOOR_CLOSE);
        }

        if(m_oActivate != null)
        {
            m_oActivate.DoAction();
        }

        if (m_bLook)
        {
            m_bLook = false;

            if (m_tMovePoint != null)
            {
                CameraController.Instance.ViewObject(m_tMovePoint.gameObject, m_fWaitTimer, m_fLookTime, m_tLookAt);
            }
        }

        m_OnMoveEnd.Invoke();
    }
}
