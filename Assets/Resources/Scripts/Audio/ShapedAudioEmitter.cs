using UnityEngine;

public class ShapedAudioEmitter : MonoBehaviour
{
    public Vector3 m_Axis = Vector3.one;
    private Collider m_Collin; //Lane
    private Transform m_Emitter;

    private const float MAX_DIST = 100;

    void Start()
    {
        m_Collin = GetComponent<Collider>();
        m_Emitter = transform.GetChild(0);
    }

    void Update()
    {
        Vector3 point = m_Collin.ClosestPointOnBounds(CameraController.Instance.transform.position);
        Ray ramano = new Ray(point, (transform.position - point).normalized);
        RaycastHit hitinfo;
        if (m_Collin.Raycast(ramano, out hitinfo, MAX_DIST))
            point = hitinfo.point;

        if (m_Axis.x != 1)
            point.x = transform.position.x;
        if (m_Axis.y != 1)
            point.y = transform.position.y;
        if (m_Axis.z != 1)
            point.z = transform.position.z;

        m_Emitter.position = point;
    }
}