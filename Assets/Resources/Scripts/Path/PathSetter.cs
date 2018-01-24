using UnityEngine;
using System.Collections;

public class PathSetter : MonoBehaviour
{
    public PathObject m_pPath;
    public PathFollower m_RequiredFollower;
    public bool m_bForceMove = false;
    public FACING_DIR m_WalkDirection;
    public FACING_DIR m_FinishDirection;
    [HideInInspector] public bool hit = false;

    void OnTriggerEnter(Collider a_col)
    {
        PathFollower follower = a_col.GetComponentInParent<PathFollower>();
        if (follower != null && (m_RequiredFollower == null? true : m_RequiredFollower == follower))
        {
            follower.SetPath(m_pPath, m_WalkDirection, m_bForceMove);
            follower.m_FinishDir = m_FinishDirection;
            hit = true;
            return;
        }
        
        //AnimalCollider col = a_col.GetComponent<
        //follower = 
        //if(follower)
        //{
        //
        //}
    }
}
