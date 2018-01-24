using UnityEngine;
using System.Collections.Generic;

public class PathObject : MonoBehaviour
{
    public Color m_cPointColor = new Color(0, 0, 1, 0.5f);
    public Color m_cLineColor = new Color(1, 0, 0);


    private List<Transform> m_lPoints;

    // Use this for initialization
    void Start()
    {
        m_lPoints = new List<Transform>();
        foreach (Transform child in transform)
        {
            m_lPoints.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_lPoints.Count != transform.childCount)
        {
            foreach (Transform child in transform)
            {
                m_lPoints.Add(child);
            }
        }
    }

    public Vector3 GetPoint(int a_index)
    {
        if (a_index < 0)
        {
            a_index = 0;
        }
        else if (a_index >= m_lPoints.Count)
        {
            a_index = m_lPoints.Count - 1;
        }

        return m_lPoints[a_index].position;
    }

    public int length
    {
        get
        {
            return m_lPoints.Count;
        }
    }

    void OnDrawGizmos()
    {
        if (transform.childCount > 0)
        {
            Vector3? m_v3LastPoint = null;
            foreach (Transform child in transform)
            {
                Gizmos.color = m_cPointColor;
                Gizmos.DrawCube(child.position, new Vector3(0.3f, 0.3f, 0.3f));

                if (m_v3LastPoint == null)
                {
                    m_v3LastPoint = child.position;
                }
                else
                {
                    Gizmos.color = m_cLineColor;
                    Gizmos.DrawLine(m_v3LastPoint.Value, child.position);
                    m_v3LastPoint = child.position;
                }
            }
        }
    }
}
