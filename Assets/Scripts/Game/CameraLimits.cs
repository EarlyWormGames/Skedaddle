using UnityEngine;
using System.Collections;

public class CameraLimits : MonoBehaviour
{
    public Vector2 m_v2XLimits = new Vector2(-1, 1);
    public Vector2 m_v2YLimits = new Vector2(-1, 1);

    // Use this for initialization
    void Start()
    {
        CameraController.Instance.m_v2XLimits = m_v2XLimits;
        CameraController.Instance.m_v2YLimits = m_v2YLimits;
    }
}
