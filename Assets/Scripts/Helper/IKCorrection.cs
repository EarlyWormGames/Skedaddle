using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class IKCorrection : MonoBehaviour {

    
    Poodle pPoodle;
    Transform PoodleT;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Poodle>())
        {
            pPoodle = other.GetComponentInParent<Poodle>();
            PoodleT = pPoodle.transform;
        }

        
        
    }


    void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<Poodle>() == pPoodle)
        {
            if (pPoodle != null && PoodleT != null)
            {
                Vector3 Dist = this.transform.position - PoodleT.position;
                float IKLedgeBlend = 1.5f - Dist.magnitude;
                pPoodle.m_IKLedgeBlend = IKLedgeBlend;
                pPoodle.m_bIKAnimationFix = true;
                //pPoodle.m_gqGrounder.solver.heightOffset = -0.1f;

                float DistToGround = pPoodle.m_gqGrounder.forelegSolver.rootHit.distance;
                Debug.Log(DistToGround);

                //cant reach the ground
                if (DistToGround < pPoodle.m_gqGrounder.forelegSolver.maxStep)
                {
                    pPoodle.m_gqGrounder.solver.lowerPelvisWeight = 0.0f;
                }
                else //able to reach the ground
                {
                    pPoodle.m_gqGrounder.solver.lowerPelvisWeight = -10.0f;

                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pPoodle.m_IKLedgeBlend = 0;
        pPoodle.m_bIKAnimationFix = false;
        pPoodle.m_gqGrounder.solver.heightOffset = 0;
        pPoodle.m_gqGrounder.solver.lowerPelvisWeight = 0.0f;
    }


}
