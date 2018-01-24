using UnityEngine;
using System.Collections.Generic;

public class PressureButton : ActionObject
{
    public Transform m_tMovePoint;
    public Transform m_tLookAt;
    public float m_fWaitTimer;
    public float m_fLookTime;

    public bool m_bRequireAnimals = true;
    public Animator m_aController;
    public Activations[] m_aObjects;
    [SerializeField] public List<Collider> m_acIgnores;

    private List<GameObject> m_lgObj;
    private bool m_bLook = true;

    //Inherited functions

    protected override void OnStart()
    {
        m_lgObj = new List<GameObject>();
        if(m_aController == null)
            m_aController = GetComponentInChildren<Animator>();
    }
    protected override void OnUpdate()
    {
        if (m_lgObj.Count > 0)
        {
            for (int i = 0; i < m_lgObj.Count; ++i)
            {
                if (m_lgObj[i] == null)
                {
                    m_lgObj.RemoveAt(i);
                    --i;
                }
            }

            if (m_lgObj.Count == 0)
            {
                for (int i = 0; i < m_aObjects.Length; ++i)
                {
                    if(!m_aObjects[i].m_bDoReverse)
                        m_aObjects[i].m_aObj.DoActionOff();
                    else
                        m_aObjects[i].m_aObj.DoActionOn();
                }
                m_aController.SetBool("Switch", false);
            }
        }
        else if(m_lgObj.Count == 0)
        {
            m_aController.SetBool("Switch", false);
        }
    }

    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }

    void OnTriggerEnter(Collider a_col)
    {
        if (m_bRequireAnimals)
        {
            Animal anim = a_col.GetComponentInParent<Animal>();
            if (anim == null)
                return;
        }
        else
        {
            if (m_acIgnores != null ? m_acIgnores.Contains(a_col) : true)
                return;
        }

        if (!m_lgObj.Contains(a_col.gameObject))
        {
            m_lgObj.Add(a_col.gameObject);
        }

        for (int i = 0; i < m_aObjects.Length; ++i)
        {
            if (m_aObjects[i].m_bDoReverse)
                m_aObjects[i].m_aObj.DoActionOff();
            else
                m_aObjects[i].m_aObj.DoActionOn();
        }
        m_aController.SetBool("Switch", true);

        if (m_bLook)
        {
            m_bLook = false;

            if (m_tMovePoint != null)
            {
                CameraController.Instance.ViewObject(m_tMovePoint.gameObject, m_fWaitTimer, m_fLookTime, m_tLookAt);
            }
        }
    }

    void OnTriggerExit(Collider a_col)
    {
        if (m_lgObj.Contains(a_col.gameObject))
        {
            m_lgObj.Remove(a_col.gameObject);
        }
        else
            return;

        for (int i = 0; i < m_aObjects.Length; ++i)
        {
            if (!m_aObjects[i].m_bDoReverse)
                m_aObjects[i].m_aObj.DoActionOff();
            else
                m_aObjects[i].m_aObj.DoActionOn();
        }
    }
    
    //protected override void DoAction() { }
}
