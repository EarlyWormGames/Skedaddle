using UnityEngine;
using System.Collections;

/// <summary>
/// draws the given cameras' frustrum
/// 
/// possibly move to helpers?
/// </summary>
public class FrustrumViewer : MonoBehaviour
{
    public Camera m_Camera;
    void OnDrawGizmos()
    {
        if (m_Camera == null)
            return;

        if (!enabled)
            return;

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawFrustum(m_Camera.transform.position, m_Camera.fieldOfView, m_Camera.farClipPlane, m_Camera.nearClipPlane, m_Camera.aspect);
    }
}
