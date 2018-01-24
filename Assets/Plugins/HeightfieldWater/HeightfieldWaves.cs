using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HeightfieldWater))]
public class HeightfieldWaves : MonoBehaviour
{
    [System.Serializable]
    public class WavePoint
    {
        public float m_TimeScale = 1f;
        public float m_Force = 1f;
        public Transform m_Point;
    }

    public WavePoint[] m_WavePoints = new WavePoint[0];
    private HeightfieldWater m_Water;

    void Start()
    {
        m_Water = GetComponent<HeightfieldWater>();      
    }

    // Update is called once per frame
    void Update()
    {
        if (m_WavePoints.Length < 1)
            return;

        foreach (WavePoint point in m_WavePoints)
        {
            if (point.m_Point == null)
                continue;
            float t = Mathf.Sin(Time.timeSinceLevelLoad * point.m_TimeScale);
            if (t == 1)
                m_Water.AddForce(point.m_Point.position, -Mathf.Clamp01(t) * point.m_Force);
        } 
    }
}
