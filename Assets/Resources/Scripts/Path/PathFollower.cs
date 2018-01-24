using UnityEngine;
using System.Collections;

public class PathFollower : MonoBehaviour
{
    public float m_fStopDist = 0.1f;

    internal Vector3 m_v3Dir;
    internal PathObject m_pPath;

    internal bool m_bFollow = false;
    internal FACING_DIR m_FinishDir;

    private int m_iPathIndex;
    private FACING_DIR m_pCurrentDir;
    private FACING_DIR m_eForcedDir;
    private RigidbodyConstraints m_rCons;
    private Animal m_Animal;

    void Start()
    {
        m_Animal = GetComponent<Animal>();
    }

    public void FollowPath(FACING_DIR a_dir, float a_speed, Rigidbody a_rig, FACING_DIR a_faceDir = FACING_DIR.NONE)
    {
        if (m_pPath != null)
        {
            if (m_pCurrentDir != a_dir)
            {
                m_pCurrentDir = a_dir;

                if (m_pCurrentDir == FACING_DIR.LEFT)
                {
                    PrevPoint();
                }
                else if (m_pCurrentDir == FACING_DIR.RIGHT)
                {
                    NextPoint();
                }
            }

            Vector3 point = m_pPath.GetPoint(m_iPathIndex);
            point.y = transform.position.y;
            float dist = Vector3.Distance(transform.position, point);
            if (dist <= m_fStopDist)
            {
                if (m_pCurrentDir == FACING_DIR.LEFT)
                {
                    PrevPoint();
                }
                else if (m_pCurrentDir == FACING_DIR.RIGHT)
                {
                    if (!NextPoint())
                    {
                        m_eForcedDir = FACING_DIR.NONE;
                        m_bFollow = false;
                        a_rig.constraints = m_rCons;
                        m_Animal.m_bCanWalkLeft = true;
                        m_Animal.m_bCanWalkRight = true;
                        m_Animal.m_bTurning = false;
                        m_pPath = null;
                        m_Animal.m_fFacingDir = m_eForcedDir;

                        if ((m_Animal.m_bTurned && m_FinishDir == FACING_DIR.RIGHT) || (!m_Animal.m_bTurned && m_FinishDir == FACING_DIR.LEFT))
                            m_Animal.Turn(m_FinishDir);

                        m_FinishDir = FACING_DIR.NONE;

                        if (GetComponent<Loris>() != null)
                            GetComponent<Loris>().m_bHorizontalRope = false;
                        return;
                    }
                }
            }

            point = m_pPath.GetPoint(m_iPathIndex);
            point.y = transform.position.y;

            Vector3 norm = ((point - transform.position) + (point - transform.position)).normalized;
            float mult = (1f - Vector3.Distance(transform.position, point)) - 0.5f;
            mult = Mathf.Max(mult, 0.5f);

            if (m_Animal)
                m_Animal.MoveInDirection(norm);
            else
                a_rig.AddForce(norm * a_speed);

            if (m_pCurrentDir == FACING_DIR.LEFT)
            {
                Vector3 prev = Vector3.zero;
                NextPoint(out prev);
                m_v3Dir = point - prev;
                m_Animal.m_bTurned = true;
            }
            else if (m_pCurrentDir == FACING_DIR.RIGHT)
            {
                Vector3 prev = Vector3.zero;
                PrevPoint(out prev);
                m_v3Dir = point - prev;
                m_Animal.m_bTurned = false;
            }

            if (a_faceDir != FACING_DIR.NONE)
            {
                if (m_pCurrentDir != a_faceDir)
                {
                    if (m_pCurrentDir == FACING_DIR.LEFT)
                    {
                        Vector3 prev = Vector3.zero;
                        NextPoint(out prev);
                        m_v3Dir = prev - point;
                    }
                    else if (m_pCurrentDir == FACING_DIR.RIGHT)
                    {
                        Vector3 prev = Vector3.zero;
                        PrevPoint(out prev);
                        m_v3Dir = prev - point;
                    }
                }
            }
        }
        else
        {
            a_rig.AddForce(a_speed * Vector3.left * (a_dir == FACING_DIR.LEFT ? 1 : -1) * 0.5f);
        }
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if (m_bFollow)
        {
            FollowPath(FACING_DIR.RIGHT, GetComponent<Animal>().m_fWalkSpeed, GetComponent<Rigidbody>());
        }
    }

    bool NextPoint()
    {
        int index = m_iPathIndex;
        ++m_iPathIndex;
        if (m_iPathIndex >= m_pPath.length)
        {
            m_iPathIndex = m_pPath.length;
        }

        if (index == m_iPathIndex)
            return false;

        return true;
    }

    void NextPoint(out Vector3 a_point)
    {
        a_point = m_pPath.GetPoint(m_iPathIndex + 1);
    }

    void PrevPoint()
    {
        --m_iPathIndex;
        if (m_iPathIndex < 0)
        {
            m_iPathIndex = 0;
        }
    }

    void PrevPoint(out Vector3 a_point)
    {
        a_point = m_pPath.GetPoint(m_iPathIndex - 1);
    }

    public FACING_DIR GetRotation()
    {
        return m_eForcedDir;
    }

    public void SetPath(PathObject a_path, FACING_DIR a_faceDir = FACING_DIR.NONE, bool m_bForce = false)
    {
        m_pPath = a_path;
        m_iPathIndex = 0;

        m_eForcedDir = a_faceDir;

        if (m_bForce)
        {
            m_Animal.m_bCanWalkLeft = false;
            m_Animal.m_bCanWalkRight = false;
            m_Animal.m_fFacingDir = m_eForcedDir;

            m_rCons = GetComponent<Rigidbody>().constraints;
            m_Animal.m_rBody.constraints = RigidbodyConstraints.FreezeRotation;
            m_iPathIndex = 1;
            m_bFollow = true;
        }
    }
}
