using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineSetter : MonoBehaviour
{
    public BezierSpline m_Spline;

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerMovement>())
        {
            other.GetComponent<PlayerMovement>().SetSpline(m_Spline);
        }
    }
}
