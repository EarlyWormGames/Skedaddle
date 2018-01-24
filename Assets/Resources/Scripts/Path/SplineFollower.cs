using UnityEngine;
using System.Collections;

public class SplineFollower : ActionObject
{
    public BezierSplineFollower m_Spline;
    public BezierSpline m_LookatSpline;

    public string m_ActionKey = "Action";

    protected Animal m_aAnimal;
    protected bool m_bMoveTo = false;
    protected bool m_bFollow = false;

    public bool m_bFirstHit = false;

    public override void DoAction()
    {
        m_bMoveTo = true;
        m_aAnimal = m_aCurrentAnimal;
        m_bFirstHit = true;
        m_Spline.OnPathEnd.AddListener(() => Finish());
    }

    protected override void OnUpdate()
    {
        if (m_aAnimal == null)
        {
            if (m_aCurrentAnimal == null)
                return;
            else if ((m_aCurrentAnimal.m_oCurrentObject != this && m_aCurrentAnimal.m_oCurrentObject != null) || !m_aCurrentAnimal.m_bSelected)
                return;

            if (Keybinding.GetKeyDown(m_ActionKey) || Controller.GetButtonDown(ControllerButtons.A))
            {
                DoAction();
            }
        }
        DoAnimation();

        if (m_bMoveTo)
        {
            if (m_aAnimal.MoveTo(m_Spline.m_Spline.GetPoint(0).x))
            {
                m_bMoveTo = false;
                m_aAnimal = m_aCurrentAnimal;
                m_aAnimal.m_rBody.isKinematic = true;
                m_aAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollide");
                m_aAnimal.m_bCheckForFall = false;
                m_aAnimal.m_bAllowAutoRotate = false;
                m_aAnimal.m_bUseIK = false;
                StartAnimation();
            }
        }

        if (m_bFollow && m_LookatSpline != null && m_aAnimal != null)
        {
            m_aAnimal.m_v3ForwardTarg = m_LookatSpline.GetPoint(m_Spline.GetTime() + (m_LookatSpline == m_Spline.m_Spline ? Time.deltaTime : 0)) - m_aAnimal.transform.position;
        }
    }

    protected virtual void StartAnimation() { StartFollow(); }
    protected virtual void OnFinish() { }

    public void Finish()
    {
        OnFinish();
        m_aAnimal.m_rBody.isKinematic = false;
        m_aAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");
        m_aAnimal.m_bCheckForFall = true;
        m_aAnimal.m_bAllowAutoRotate = true;
        m_aAnimal.m_bUseIK = true;
        m_aAnimal = null;
        m_Spline.m_MoveObject = null;
        m_bFollow = false;
    }

    public void StartFollow()
    {
        Vector3 point = m_Spline.m_Spline.GetControlPointWorld(0);
        //point.y = m_aAnimal.transform.position.y;
        m_Spline.m_Spline.SetControlPointWorld(0, point);
        m_aAnimal.m_v3ForwardTarg = point - m_aAnimal.transform.position;

        m_Spline.m_MoveObject = m_aAnimal.transform;
        m_Spline.Follow();

        m_bFollow = true;
    }
}
