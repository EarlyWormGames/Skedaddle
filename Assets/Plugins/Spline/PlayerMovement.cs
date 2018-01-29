using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public BezierSpline m_Spline;
    public float m_Speed = 10.0f;

    private float m_SplinePos;
    private bool enter;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_Spline != null)
        {
            if (!enter)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    m_SplinePos -= Time.deltaTime * m_Speed;

                    if (m_SplinePos < 0)
                    {
                        m_Spline = null;
                        m_SplinePos = 0;
                        return;
                    }
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    m_SplinePos += Time.deltaTime * m_Speed;

                    if (m_SplinePos > m_Spline.MaxSplineLength)
                    {
                        m_Spline = null;
                        m_SplinePos = 0;
                        return;
                    }
                }

                float t = m_Spline.GetArcDist(m_SplinePos);

                Vector3 pos = m_Spline.GetPoint(t);
                pos.y = transform.position.y;

                if (Vector3.Distance(transform.position, pos) > 0.1f)
                {
                    transform.position += (pos - transform.position).normalized * m_Speed * Time.deltaTime;
                }
            }
            else
            {
                float t = m_Spline.GetArcDist(m_SplinePos);

                Vector3 pos = m_Spline.GetPoint(t);
                pos.y = transform.position.y;

                if (Vector3.Distance(transform.position, pos) < m_Speed)
                {
                    enter = false;
                    return;
                }

                transform.position += (pos - transform.position).normalized * m_Speed * Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-Time.deltaTime * m_Speed, 0, 0);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Time.deltaTime * m_Speed, 0, 0);
            }
        }
    }

    public void SetSpline(BezierSpline spline)
    {
        m_Spline = spline;
        m_SplinePos = spline.GetArcLength(transform.position);
        enter = true;
    }
}
