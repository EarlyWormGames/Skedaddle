using UnityEngine;
using System.Collections;

/// <summary>
/// GUI Videos.
/// </summary>
public class ExtrasVideo : ExtrasItem
{
    public GameObject m_gPlayParent;
    public MeshRenderer m_vPlayGUI;
    public float m_fLerpSpeed = 1f;
    public Transform m_tStartPoint;
    public Transform m_tEndPoint;
    public AnimationCurve m_aCurve;

    private float m_fLerpTimer = -1f;

    public override void Show()
    {
        base.Show();
        m_bSelected = true;
        m_vPlayGUI.enabled = false;
        m_fLerpTimer = 0f;
        m_bCanQuit = false;
    }

    public override void Hide()
    {
        base.Hide();
        m_bSelected = false;
        m_vPlayGUI.enabled = true;
        m_fLerpTimer = 0f;
    }

    protected override void OnUpdate()
    {
        if (m_fLerpTimer < 0f)
            return;

        float percent = m_aCurve.Evaluate(m_fLerpTimer / m_fLerpSpeed);
        m_fLerpTimer += Time.deltaTime;

        if (percent > 1f)
        {
            m_fLerpTimer = -1f;
            percent = 1f;
            m_bCanQuit = true;
        }

        m_gPlayParent.transform.position = Vector3.Lerp(m_bSelected ? m_tStartPoint.position : m_tEndPoint.position, m_bSelected ? m_tEndPoint.position : m_tStartPoint.position, percent);
        m_gPlayParent.transform.rotation = Quaternion.Lerp(m_bSelected ? m_tStartPoint.rotation : m_tEndPoint.rotation, m_bSelected ? m_tEndPoint.rotation : m_tStartPoint.rotation, percent);
    }
}
