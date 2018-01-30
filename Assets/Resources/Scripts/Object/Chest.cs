using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class Chest : ActionObject
{
    //==================================
    //          Public Vars
    //==================================

    public bool         m_bResetChest = false;

    public GameObject   m_goPeanutJoint;

    public Light        m_lPeanutHalo;
    public Light        m_lPeanutLight;

    public Material     m_matLock;

    public ParticleSystem m_psPlus1;

    public float        m_fRotateSpeed = 2f;

    public string       LandEvent;

    //==================================
    //          Internal Vars
    //==================================

    internal Animator   m_aChestAnimator;

    internal bool       m_bOpened = false;
    internal bool       m_bRattle = false;
    internal float      m_fRattleTimerMax;
    internal float      m_fRattleActualTime;

    internal bool       m_bRotate = false;

    //==================================
    //          Private Vars
    //================================== 
    private float       m_fCurrentRot = 0f;

    //Inherited functions

    protected override void OnStart()
    {
        m_aChestAnimator = GetComponentInParent<Animator>();
        ResetRattle();
        m_bOpened = false;
        m_fCurrentRot = 0f;

        if (GameManager.IsDebugScene(SceneManager.GetActiveScene().name))
            return;

        //string[] al = SceneManager.GetActiveScene().name.Split('-');
        //if (SaveManager.CheckPeanut(Convert.ToInt32(al[0]), Convert.ToInt32(al[1])))
        //{
        //    m_aChestAnimator.SetBool("Start_Opened", true);
        //    m_matLock.color = new Color(1, 1, 1, 0);
        //    enabled = false;
        //}
    }

    protected override void OnUpdate()
    {
        //Debug Reset
        if(m_bResetChest)
        {
            ResetRattle();
            m_bResetChest = false;
        }

        m_lPeanutHalo.transform.position = new Vector3(m_goPeanutJoint.transform.position.x, m_goPeanutJoint.transform.position.y, m_goPeanutJoint.transform.position.z + 0.05f);
        m_matLock.color = new Color(1, 1, 1, m_aChestAnimator.GetFloat("LockAlpha"));

        if (!m_bOpened)
        {
            if (m_aCurrentAnimal != null)
            {
                if (m_aCurrentAnimal.m_oCurrentObject == this || m_aCurrentAnimal.m_oCurrentObject == null)
                {
                    SetTimer(EWEyeTracking.GetFocusedObject() == m_aCurrentAnimal.m_GazeObject || EWEyeTracking.GetFocusedObject() == m_GazeObject);

                    if (m_aCurrentAnimal.m_bSelected)
                    {
                        if ((Keybinding.GetKey("Action") || Controller.GetButtonDown(ControllerButtons.A) || m_fGazeTimer >= EWEyeTracking.shortHoldTime))
                        {
                            DoAction();
                        }
                    }
                }
            }

            if (m_bRattle)
            {
                m_aChestAnimator.SetBool("Rattle", false);
                m_bRattle = false;
            }
            else
            {
                m_fRattleActualTime -= Time.deltaTime;

                if (m_fRattleActualTime <= 0.0f)
                {
                    m_aChestAnimator.SetBool("Rattle", true);
                    m_bRattle = true;
                    ResetRattle();
                }
            }

            m_lPeanutHalo.range = 0;
            m_lPeanutHalo.intensity = 0;
            m_lPeanutLight.intensity = 0;
        }
        else
        {
            m_lPeanutHalo.range = m_aChestAnimator.GetFloat("HaloRange");
            m_lPeanutHalo.intensity = m_aChestAnimator.GetFloat("HaloIntensity");
            m_lPeanutLight.intensity = m_aChestAnimator.GetFloat("LightIntensity");

        }
    }

    void FixedUpdate()
    {
        if (m_bRotate)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z -= m_fRotateSpeed;
            m_fCurrentRot -= m_fRotateSpeed;
            transform.rotation = Quaternion.Euler(rot);
        }

        if (m_fCurrentRot > 360f || m_fCurrentRot < -360f)
        {
            m_fCurrentRot = 0f;
            m_bRotate = false;
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z = 0;
            transform.rotation = Quaternion.Euler(rot);
        }
    }

    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        m_aChestAnimator.SetBool("Opened", true);
        m_bOpened = true;

        Analytics.CustomEvent("Peanut Chest Open", new Dictionary<string, object>
        {
            { "Level", SceneManager.GetActiveScene().name }
        });

        if (GameManager.IsDebugScene(SceneManager.GetActiveScene().name))
            return;

        string[] al = SceneManager.GetActiveScene().name.Split('-');
        SaveManager.UnlockPeanut(Convert.ToInt32(al[0]), Convert.ToInt32(al[1]));
    }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }

    public void PlayParticle()
    {
        m_psPlus1.Play();
    }

    protected void ResetRattle()
    {
        m_fRattleTimerMax = UnityEngine.Random.Range(3.0f, 5.0f);
        m_fRattleActualTime = m_fRattleTimerMax;
    }
}
