using UnityEngine;
using UnityEngine.Events;
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
    [Range(0, 1)] public float m_fHeadWeight;
    public float m_BridgeTongueScale;
    [Range(0, 0.5f)]public float m_BridgeEndAnimationStart;


    public UnityEvent OnBuildBridge, OnRemoveBridge;


    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //==================================
    private Transform[] m_tJoints;
    private bool m_bMoveTo;
    private bool m_bBridgeMade;
    private float m_fTimer;
    private TerrainDetection m_headTerrain;
    private AimIK m_headIK;
    private CCDIK m_TongueIK;

    //Inherited functions

    protected override void OnStart()
    {
        m_TongueIK = m_goBridge.GetComponent<CCDIK>();
        m_tJoints = m_aTongue.GetComponentsInChildren<Transform>();
        m_goBridge.SetActive(false);

        m_bBlocksMovement = true;
        m_bBlocksTurn = true;
        m_CanBeDetached = false;
        m_CanDetach = false;
    }

    protected override void OnUpdate()
    {
        if (m_aCurrentAnimal == null)
            return;

        if (m_fTimer < 1)
        {
            m_fTimer += Time.deltaTime;
        }

        if (m_bBridgeMade)
        {
            if(m_aCurrentAnimal.GetComponent<Anteater>() != null)
            {
                m_aTongue.transform.position = m_aCurrentAnimal.GetComponent<Anteater>().m_tTongueEnd.position;
            }

            m_TongueIK.solver.IKPositionWeight = Mathf.Lerp(0, 1, m_fTimer * 2);


            foreach (Transform x in m_tJoints)
            {
                x.localScale = new Vector3(Mathf.Lerp(0, m_BridgeTongueScale, m_fTimer * 2), 1, 1);
                if(x.localScale.x >= m_BridgeTongueScale) m_TongueIK.fixTransforms = false;
            }
            if (m_fTimer >= 0.2f)
            {
                
                //if (m_headTerrain != null)
                //    m_headTerrain.overrideTarget = false;
                //m_headIK.solver.IKPositionWeight = Mathf.Lerp(m_headIK.solver.IKPositionWeight, m_fHeadWeight, Time.deltaTime * 3);
                //m_headIK.solver.target.transform.position = m_tJoints[3].transform.position;
            }
        }
        else if (m_goBridge.activeInHierarchy)
        {
            m_TongueIK.fixTransforms = true;
            if (m_aCurrentAnimal.GetComponent<Anteater>() != null)
            {
                m_aTongue.transform.position = m_aCurrentAnimal.GetComponent<Anteater>().m_tTongueEnd.position;
            }
            m_TongueIK.solver.IKPositionWeight = 0;
            foreach (Transform x in m_tJoints)
            {
                x.localScale = new Vector3(Mathf.Lerp(1, 0, m_fTimer * 2), 1, 1);
            }
            if (m_fTimer >= m_BridgeEndAnimationStart)
            {
                m_aCurrentAnimal.m_aAnimalAnimator.SetBool("TongueBridge", false);
                //m_headTerrain.overrideTarget = true;
            }
            if (m_fTimer >= 0.5f)
            {
                
                m_goBridge.SetActive(false);
                m_aCurrentAnimal.m_bCanWalkLeft = true;
                m_aCurrentAnimal.m_bCanWalkRight = true;
                
                m_aCurrentAnimal.m_oCurrentObject = null;
                m_aCurrentAnimal = null;

                m_headTerrain = null;
                m_headIK = null;
            }

        }
        else
        {
            foreach (Transform x in m_tJoints)
            {
                x.localScale = new Vector3(0, 1, 1);
            }
        }
    }

    protected override void OnCanTrigger()
    {
        if (input.interact.wasJustPressed)
        {
            DoAction();
        }
    }

    public override void DoAction()
    {
        if (m_bBridgeMade)
        {
            m_bBridgeMade = false;
            m_fTimer = 0;

            OnRemoveBridge.Invoke();
        }
        else
        {
            m_aCurrentAnimal = Animal.CurrentAnimal;
            m_aCurrentAnimal.m_oCurrentObject = this;
            m_aCurrentAnimal.m_aAnimalAnimator.SetTrigger("TongueStart");
            m_aCurrentAnimal.m_aAnimalAnimator.SetBool("TongueBridge", true);
            m_aCurrentAnimal.m_bCanWalkLeft = false;
            m_aCurrentAnimal.m_bCanWalkRight = false;

            m_TongueIK.fixTransforms = true;

            OnBuildBridge.Invoke();

            //m_headTerrain = m_aCurrentAnimal.m_tCollider.Find("Head Offset").GetComponent<TerrainDetection>();
            //m_headIK = (AimIK)m_headTerrain.effectedIK;
        }
    }

    public void BuildBridge()
    {
        m_goBridge.SetActive(true);
        m_bBridgeMade = true;
        m_fTimer = 0;
    }
}