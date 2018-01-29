using UnityEngine;
using RootMotion.FinalIK;
using System.Collections;

public class BridgeMaker : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public GameObject m_goBridge;
    public Transform m_tMovePoint;
    public GameObject m_aTongue;
    [Range(0,1)] public float m_fHeadWeight;
    

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //==================================
    private Transform[] m_tJoints; 
    private Animal m_Annie;
    private bool m_bMoveTo;
    private bool m_bBridgeMade;
    private float m_fTimer;
    private TerrainDetection m_headTerrain;
    private AimIK m_headIK;

    //Inherited functions

    protected override void OnStart()
    {
        m_tJoints = m_aTongue.GetComponentsInChildren<Transform>();
        m_goBridge.SetActive(false);
        
    }

    protected override void OnUpdate()
    {
        if (m_fTimer < 1)
        {
            m_fTimer += Time.deltaTime;
        }
        
        if (m_bBridgeMade)
        {
            foreach (Transform x in m_tJoints)
            {
                x.localScale = new Vector3(Mathf.Lerp(0, 1, m_fTimer * 2), 1, 1);
            }
            if (m_fTimer >= 0.2f)
            {
                if (m_headTerrain != null)
                    m_headTerrain.overrideTarget = false;
                m_headIK.solver.IKPositionWeight = Mathf.Lerp(m_headIK.solver.IKPositionWeight, m_fHeadWeight, Time.deltaTime * 3);
                m_headIK.solver.target.transform.position = m_tJoints[3].transform.position;
            }
        }
        else if (m_goBridge.activeInHierarchy)
        {
            foreach (Transform x in m_tJoints)
            {
                x.localScale = new Vector3(Mathf.Lerp(1, 0, m_fTimer * 2), 1, 1);
            }
            if(m_fTimer >= 0.7f)
            {
                m_headTerrain.overrideTarget = true;
            }
            if(m_fTimer >= 1)
            {
                m_Annie.m_bCanWalkLeft = true;
                m_Annie.m_bCanWalkRight = true;
                m_Annie.m_aAnimalAnimator.SetBool("TongueBridge", false);
                m_goBridge.SetActive(false);
                m_Annie = null;
            }
            
        } else
        {
            foreach (Transform x in m_tJoints)
            {
                x.localScale = new Vector3(0, 1, 1);
            }
        }

        if (m_aCurrentAnimal != null)
        {
            if (m_aCurrentAnimal.m_bSelected)
            {
                if ((Keybinding.GetKeyDown("Action") || Controller.GetButtonDown(ControllerButtons.A)))
                {
                    m_bMoveTo = true;
                }
            }
        }

        if (m_bMoveTo)
        {
            if (m_aCurrentAnimal.MoveTo(m_tMovePoint.position.x))
            {
                m_bMoveTo = false;
                DoAction();
            }
        }
    }

    public override void DoAction()
    {
        if (m_bBridgeMade)
        {
            m_bBridgeMade = false;
            m_fTimer = 0;
        }
        else
        {
            m_Annie = m_aCurrentAnimal;
            m_Annie.m_aAnimalAnimator.SetBool("TongueBridge", true);
            m_Annie.m_bCanWalkLeft = false;
            m_Annie.m_bCanWalkRight = false;
        }
    }

    public void BuildBridge()
    {
        m_goBridge.SetActive(true);
        m_bBridgeMade = true;
        m_fTimer = 0;
    }

    public override void AnimalEnter(Animal a_animal)
    {
        a_animal.m_eExtras.m_bmTongueBridge = this;
        m_headTerrain = a_animal.m_tCollider.Find("Head Offset").GetComponent<TerrainDetection>();
        m_headIK = (AimIK)m_headTerrain.effectedIK;
    }

    protected override void AnimalExit(Animal a_animal)
    {
        a_animal.m_eExtras.m_bmTongueBridge = null;
        m_headTerrain = null;
        m_headIK = null;
    }
}
