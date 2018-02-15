using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class CutsceneObject : MonoBehaviour
{
    //==================================
    //          Public Vars
    //==================================
    public Component[]      m_aDisableComps;
    public Rigidbody        m_rBody;
    //public Collider         m_cCol;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Protected Vars
    //================================== 
    protected bool m_bInScene = false;

    //==================================
    //          Private Vars
    //==================================


    // Use this for initialization
    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();
    }

    public void SceneStart()
    {
        m_bInScene = true;
        //////////////////////////////////
        //Disable all this stuff
        for (int i = 0; i < m_aDisableComps.Length; ++i)
        {
            if (m_aDisableComps[i] != null)
            {
                //Check that it has the enabled get/set in the component
                if (m_aDisableComps[i].GetType().GetProperty("enabled") != null)
                {
                    object[] setEnabled = new object[1];
                    setEnabled[0] = false;
                    //Double check
                    bool? valerie = m_aDisableComps[i].GetType().InvokeMember("enabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, Type.DefaultBinder, m_aDisableComps[i], null) as bool?;
                    if (valerie != null)
                    {
                        m_aDisableComps[i].GetType().InvokeMember("enabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, m_aDisableComps[i], setEnabled);
                    }
                }
            }
        }
        if (m_rBody != null)
        {
            m_rBody.isKinematic = true;
        }
        //////////////////////////////////
        OnSceneStart();
    }

    public void SceneEnd()
    {
        m_bInScene = false;
        //////////////////////////////////
        //Re-enable all this stuff
        for (int i = 0; i < m_aDisableComps.Length; ++i)
        {
            //Check that it has the enabled get/set in the component
            if (m_aDisableComps[i].GetType().GetProperty("enabled") != null)
            {
                object[] setEnabled = new object[1];
                setEnabled[0] = true;
                //Double check
                bool? valerie = m_aDisableComps[i].GetType().InvokeMember("enabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, Type.DefaultBinder, m_aDisableComps[i], null) as bool?;
                if (valerie != null)
                {
                    m_aDisableComps[i].GetType().InvokeMember("enabled", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder, m_aDisableComps[i], setEnabled);
                }
            }
        }
        if (m_rBody != null)
        {
            m_rBody.isKinematic = true;
        }
        //////////////////////////////////
        OnSceneEnd();
    }

    //////////////////////////////////
    //Virtual functions
    protected virtual void OnStart() { }
    protected virtual void OnUpdate() { }

    protected virtual void OnSceneStart() { }
    protected virtual void OnSceneEnd() { }

    protected virtual void TimelineHit() { }
    protected virtual void TimelineExit() { }

    public virtual void SetAnimParam<T>(T a_value) { }
    //////////////////////////////////

    //////////////////////////////////
    //Get and set
    public virtual Vector3? position
    {
        get
        {
            return transform.position;
        }
        set
        {
            if (value.HasValue)
            {
                transform.position = value.Value;
            }
        }
    }

    public virtual Quaternion? rotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            if (value.HasValue)
            {
                transform.rotation = value.Value;
            }
        }
    }

    public virtual float? opacity
    {
        get
        {
            return null;
        }
        set
        {
            //HEHEHEHHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHEHHEHEH
        }
    }
    //////////////////////////////////
}
