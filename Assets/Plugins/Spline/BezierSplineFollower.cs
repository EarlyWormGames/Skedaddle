using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum BezierControlPointMode
{
    Free,
    Aligned,
    Mirrored
}

public class BezierSplineFollower : MonoBehaviour
{
    public enum eLoopType
    {
        None,
        Loop,
        PingPong
    }

    public BezierSpline m_Spline;
    public Transform m_MoveObject;
    public float m_FollowTime = 5;
    public AnimationCurve m_Curve = AnimationCurve.Linear(0, 0, 1, 1);
    public eLoopType m_LoopType;
    public int m_LoopTimes = 5;
    public bool m_FollowOnStart = false;
    public bool m_Lookat = false;

    public UnityEvent OnPathEnd;


    private int m_iLoopCount = 0;
    private float m_fTime = 0;
    private bool m_bRunning = false;
    private bool m_bReverse = false;

    void Start()
    {
        if (m_FollowOnStart) Follow();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bRunning)
        {
            m_fTime += Time.deltaTime;
            float t = GetTime();

            if (m_Lookat)
                m_MoveObject.LookAt(m_Spline.GetPoint(t));
            else
                m_MoveObject.position = m_Spline.GetPoint(t);

            if (m_fTime >= m_FollowTime)
            {
                switch (m_LoopType)
                {
                    case eLoopType.None:
                        {
                            m_bRunning = false;
                            m_fTime = 0;
                            OnPathEnd.Invoke();
                            break;
                        }
                    case eLoopType.Loop:
                        {
                            ++m_iLoopCount;
                            if (m_iLoopCount >= m_LoopTimes)
                            {
                                m_bRunning = false;
                                m_fTime = 0;
                                OnPathEnd.Invoke();
                            }
                            else
                            {
                                m_fTime = 0f;
                            }
                            break;
                        }
                    case eLoopType.PingPong:
                        {
                            ++m_iLoopCount;
                            if (m_iLoopCount >= m_LoopTimes)
                            {
                                m_bRunning = false;
                                m_fTime = 0;
                                OnPathEnd.Invoke();
                            }
                            else
                            {
                                m_fTime = 0f;
                                m_bReverse = !m_bReverse;
                            }
                            break;
                        }
                }
            }
        }
    }

    public void Follow()
    {
        m_bRunning = true;
        m_iLoopCount = 0;
        m_fTime = 0;
    }

    void OnDrawGizmosSelected()
    {
        if (m_Curve == null || m_Spline == null)
            return;

        float t = m_fTime / m_FollowTime;
        t = m_Curve.Evaluate(m_bReverse ? 1 - t : t);

        Gizmos.color = new Color(0, 0.8f, 0, 0.3f);
        Gizmos.DrawCube(m_Spline.GetPoint(t), Vector3.one * 0.3f);
    }

    public float GetTime()
    {
        float t = m_fTime / m_FollowTime;
        t = m_Curve.Evaluate(m_bReverse ? 1 - t : t);
        return t;
    }
}