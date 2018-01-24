using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Cannon : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public float m_fRotSpeed = 3f;
    public float m_fLerpSpeed = 2f;
    public Vector3 m_v3RightEndRot;
    public Vector3 m_v3LeftEndRot;
    public Transform m_tRotObject;
    public Transform m_tLerpPoint;
    public Transform m_tRightShootDir;
    public Transform m_tLeftShootDir;
    public Transform m_tLorisTrail;
    public SpriteRenderer[] m_sDirectionalArrows;
    public float m_fRightShootForce = 40f;
    public float m_fLeftShootForce = 40f;
    public float m_fWaitTime = 1f;
    public ParticleSystem m_psCannonDust;

    public TutBox m_tBox;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 
    private Quaternion m_qStartRot;
    private Quaternion m_qRightEndRot;
    private Quaternion m_qLeftEndRot;
    private bool m_bStartShoot;
    private bool m_bShoot;
    private bool m_bAimLeft = false;
    private float m_fTimer;
    private RigidbodyConstraints m_rCons;
    private Animal m_aAnimal;

    private bool m_bStopped;
    private bool m_bPlayed;
    private bool m_bCanShoot = false;
    //Inherited functions

    protected override void OnStart()
    {
        m_aRequiredAnimal = ANIMAL_NAME.LORIS;
        m_qStartRot = m_tRotObject.localRotation;
    }

    protected override void OnUpdate()
    {
        m_qRightEndRot = Quaternion.Euler(m_v3RightEndRot);
        m_qLeftEndRot = Quaternion.Euler(m_v3LeftEndRot);
        if (m_aCurrentAnimal != null)
        {
            if ((m_aCurrentAnimal.m_oCurrentObject == this || m_aCurrentAnimal.m_oCurrentObject == null) || !m_aCurrentAnimal.m_bSelected)
            {
                if (m_aCurrentAnimal.m_bCanWalkLeft || m_aCurrentAnimal.m_bCanWalkRight)
                {
                    DoAction();
                }
            }
        }

        if (!m_bStartShoot)
        {
            m_tRotObject.localRotation = Quaternion.Lerp(m_tRotObject.localRotation, m_qStartRot, Time.deltaTime * 0.2f * m_fRotSpeed);
            for(int i = 0; i < m_sDirectionalArrows.Length; i++)
            {
                m_sDirectionalArrows[i].enabled = false;
            }
        }
        else
        {
            if (m_tBox != null)
            {
                m_tBox.Trigger();
            }

            m_fTimer -= Time.deltaTime;
            ((Loris)m_aCurrentAnimal).m_bInCannon = true;
            m_aCurrentAnimal.m_bCanWalkLeft = false;
            m_aCurrentAnimal.m_bCanWalkRight = false;
            m_aCurrentAnimal.m_rBody.useGravity = false;
            m_aCurrentAnimal.m_rBody.velocity = Vector3.zero;
            m_aCurrentAnimal.transform.position = Vector3.Lerp(m_aCurrentAnimal.transform.position, m_tLerpPoint.position, Time.deltaTime * m_fLerpSpeed);
            if (m_fTimer < 0)
            {
                m_tRotObject.localRotation = Quaternion.Lerp(m_tRotObject.localRotation, m_bAimLeft ? m_qLeftEndRot : m_qRightEndRot, Time.deltaTime * m_fRotSpeed);
            }
            m_aCurrentAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollide");
            m_aCurrentAnimal.StopMoveTo();
            if (Vector3.Distance(m_tRotObject.localRotation.eulerAngles, m_bAimLeft ? m_qLeftEndRot.eulerAngles : m_qRightEndRot.eulerAngles) < 1.2f)
            {
                if (!m_bStopped)
                    PlaySound(SOUND_EVENT.CANNON_ROTATE_STOP);
                m_bStopped = true;
                m_bPlayed = false;
                m_bCanShoot = true;
            }
            else if (!m_bPlayed)
            {
                m_bStopped = false;
                m_bPlayed = true;
                PlaySound(SOUND_EVENT.CANNON_ROTATE);
                m_bCanShoot = false;
            }
            if (Keybinding.GetKeyDown("MoveLeft") || Controller.GetDpadDown(ControllerDpad.Left) || Controller.GetStickPositionDown(true, ControllerDpad.Left))
            {
                m_bAimLeft = true;
            }
            if (Keybinding.GetKeyDown("MoveRight") || Controller.GetDpadDown(ControllerDpad.Right) || Controller.GetStickPositionDown(true, ControllerDpad.Right))
            {
                m_bAimLeft = false;
            }
            for (int i = 0; i < m_sDirectionalArrows.Length; i++)
            {
                if (i == 0)
                {
                    m_sDirectionalArrows[i].color = Color.Lerp(m_sDirectionalArrows[i].color, 
                        new Color(m_sDirectionalArrows[i].color.r, m_sDirectionalArrows[i].color.g, m_sDirectionalArrows[i].color.b, m_bAimLeft ? 1 : 0.5f), 
                        Time.deltaTime * 2);
                }
                else
                {
                    m_sDirectionalArrows[i].color = Color.Lerp(m_sDirectionalArrows[i].color,
                        new Color(m_sDirectionalArrows[i].color.r, m_sDirectionalArrows[i].color.g, m_sDirectionalArrows[i].color.b, m_bAimLeft ? 0.5f : 1f),
                        Time.deltaTime * 2);
                }
                m_sDirectionalArrows[i].enabled = true;
            }
            if (m_bStopped && m_aCurrentAnimal.m_bSelected)
            {
                if ((Keybinding.GetKeyDown("MoveUp") || Keybinding.GetKeyDown("Action") || Controller.GetDpadDown(ControllerDpad.Up) || Controller.GetStickPositionDown(true, ControllerDpad.Up) || Controller.GetButtonDown(ControllerButtons.A) || m_bEyetrackSelected) && m_bCanShoot)
                {
                    //SHOOT
                    m_aAnimal = m_aCurrentAnimal;
                    m_bStartShoot = false;
                    m_bShoot = true;
                }
            }
        }

        if ((Loris)m_aAnimal != null)
        {
            if (((Loris)m_aAnimal).m_bInCannon)
            {
                m_tLorisTrail.position = ((Loris)m_aAnimal).transform.position;
            }
        }
    }

    void FixedUpdate()
    {
        if (m_bShoot)
        {
            m_psCannonDust.Play();
            m_rCons = m_aAnimal.m_rBody.constraints;
            m_aAnimal.m_rBody.constraints = RigidbodyConstraints.FreezeRotation;
            m_aAnimal.m_rBody.useGravity = true;
            m_aAnimal.m_rBody.AddForce((m_bAimLeft ? m_tLeftShootDir.position - transform.position : m_tRightShootDir.position - transform.position).normalized * (m_bAimLeft ? m_fLeftShootForce : m_fRightShootForce), ForceMode.Impulse);
            m_bShoot = false;

            PlaySound(SOUND_EVENT.CANNON_SHOOT);
        }
    }

    public override void AnimalEnter(Animal a_animal)
    {
        if (m_aCurrentAnimal != null)
        {
            if (m_aCurrentAnimal.m_oCurrentObject == null && m_aCurrentAnimal.m_bSelected)
            {
                DoAction();
                m_aCurrentAnimal.m_rBody.velocity = Vector3.zero;
            }
        }
    }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        m_fTimer = m_fWaitTime;
        m_bStartShoot = true;
        m_aCurrentAnimal.m_oCurrentObject = this;
    }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }

    public void End()
    {
        if (m_aAnimal == null)
            return;

        ((Loris)m_aAnimal).m_bInCannon = false;
        m_aAnimal.m_rBody.constraints = m_rCons;
        m_aAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");
        m_aAnimal.m_bCanWalkLeft = true;
        m_aAnimal.m_bCanWalkRight = true;
        m_aAnimal.m_oCurrentObject = null;
        m_aAnimal = null;
    }
}
