using UnityEngine;
using UnityEngine.Events;
using RootMotion.FinalIK;
using System.Collections;

public class BridgeMaker : AttachableInteract
{
    //==================================
    //          Public Vars
    //==================================
    public GameObject m_goBridge;
    public Transform m_tMovePoint;
    public Transform m_tBridgeAnchor;
    public GameObject m_aTongue;
    [Range(0, 1)] public float m_fHeadWeight;
    public float m_BridgeTongueOriginalDistance;
    [Range(0, 0.5f)]public float m_BridgeEndAnimationStart;
    public bool m_IsVertical;

    public UnityEvent OnBuildBridge, OnRemoveBridge;


    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //==================================
    private Transform[] m_tJoints;
    private Vector3[] m_vJointOrigScale;
    private float m_tBridgeMulti;
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
        m_tJoints = new Transform[m_TongueIK.solver.bones.Length];
        for(int i = 0; i < m_TongueIK.solver.bones.Length; i++)
        {
            m_tJoints[i] = m_TongueIK.solver.bones[i].transform;
        }
        m_vJointOrigScale = new Vector3[m_tJoints.Length];
        for (int i = 0; i < m_tJoints.Length; i++)
        {
            m_vJointOrigScale[i] = m_tJoints[i].localScale;
        }
        m_goBridge.SetActive(false);

        BlocksMovement = true;
        BlocksTurn = true;
        CanDetach = false;

        HeadTriggerOnly = false;

        RequiredAnimal = ANIMAL_NAME.ANTEATER;
    }

    protected override void OnUpdate()
    {
        if (AttachedAnimal == null)
            return;

        if (m_fTimer < 1)
        {
            m_fTimer += Time.deltaTime;
        }

        if (m_bBridgeMade)
        {
            //The code below positions the IK's correctly
            if (AttachedAnimal.GetType().IsAssignableFrom(typeof(Anteater)))
            {
                m_aTongue.transform.position = AttachedAnimal.GetComponent<Anteater>().m_tTongueEnd.position;
            }
            m_tBridgeMulti = Vector3.Distance(m_aTongue.transform.position, m_tBridgeAnchor.position) / m_BridgeTongueOriginalDistance;

            m_TongueIK.solver.IKPositionWeight = Mathf.Lerp(0, 1, m_fTimer * 2);
            for (int i = 0; i < m_tJoints.Length; i++)
            {
                if (i == 0)
                {
                    m_tJoints[0].localScale = Vector3.Lerp(new Vector3(0, 1, 1), new Vector3(m_vJointOrigScale[i].x * m_tBridgeMulti, m_vJointOrigScale[i].y, m_vJointOrigScale[i].z), m_fTimer * 2);
                }
                else if (i == m_tJoints.Length - 1)
                {
                    m_tJoints[i].localScale = Vector3.Lerp(new Vector3(0, 1, 1), new Vector3(m_vJointOrigScale[i].x * (1 / m_tBridgeMulti), m_vJointOrigScale[i].y, m_vJointOrigScale[i].z), m_fTimer * 2);
                }
                else
                {
                    m_tJoints[i].localScale = Vector3.Lerp(new Vector3(0, 1, 1), m_vJointOrigScale[i], m_fTimer * 2);
                }
            }
            if (m_tJoints[0].localScale.x >= m_vJointOrigScale[0].x * m_tBridgeMulti) m_TongueIK.fixTransforms = false;
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
            //Undo the joints over time
            if (AttachedAnimal.GetType().IsAssignableFrom(typeof(Anteater)))
            {
                m_aTongue.transform.position = AttachedAnimal.GetComponent<Anteater>().m_tTongueEnd.position;
            }
            m_tBridgeMulti = Vector3.Distance(m_aTongue.transform.position, m_tBridgeAnchor.position) / m_BridgeTongueOriginalDistance;
            m_TongueIK.fixTransforms = true;
            if (AttachedAnimal.GetType().IsAssignableFrom(typeof(Anteater)))
            {
                m_aTongue.transform.position = AttachedAnimal.GetComponent<Anteater>().m_tTongueEnd.position;
            }
            m_TongueIK.solver.IKPositionWeight = 0;
            for (int i = 0; i < m_tJoints.Length; i++)
            {
                m_tJoints[i].localScale = Vector3.Lerp(m_vJointOrigScale[i] * m_tBridgeMulti, new Vector3(0, 1, 1), m_fTimer * 2);
            }
            if (m_fTimer >= m_BridgeEndAnimationStart)
            {
                AttachedAnimal.m_aAnimalAnimator.SetBool("TongueBridge", false);
                AttachedAnimal.m_aAnimalAnimator.SetBool("TongueRope", false);
                //m_headTerrain.overrideTarget = true;
            }
            if (m_fTimer >= 0.5f)
            {
                Detach(this);
            }

        }
        else
        {
            //Unscale the joints
            foreach (Transform x in m_tJoints)
            {
                x.localScale = new Vector3(0, 1, 1);
            }
        }
    }

    protected override void DoInteract(Animal caller)
    {
        if (m_bBridgeMade)
        {
            //Undo creating the bridge
            m_bBridgeMade = false;
            m_fTimer = 0;

            //Remove the bridge
            OnRemoveBridge.Invoke();
        }
        else
        {
            Attach(caller);

            //Setup some initial animation values
            AttachedAnimal.m_aAnimalAnimator.SetTrigger("TongueStart");
            if (!m_IsVertical)
            {
                AttachedAnimal.m_aAnimalAnimator.SetBool("TongueBridge", true);
            }
            else
            {
                AttachedAnimal.m_aAnimalAnimator.SetBool("TongueRope", true);
            }

            //Prevent the animal from moving
            AttachedAnimal.m_bCanWalkLeft = false;
            AttachedAnimal.m_bCanWalkRight = false;

            m_TongueIK.fixTransforms = true;

            OnBuildBridge.Invoke();

            //m_headTerrain = m_aCurrentAnimal.m_tCollider.Find("Head Offset").GetComponent<TerrainDetection>();
            //m_headIK = (AimIK)m_headTerrain.effectedIK;
        }
    }

    /// <summary>
    /// Callback from the animator to make the invisible bridge active
    /// </summary>
    public void BuildBridge()
    {
        //Bridge should be fully built, 
        m_goBridge.SetActive(true);
        m_bBridgeMade = true;
        m_fTimer = 0;
    }

    protected override void OnDetaching(Animal animal)
    {
        //Disable the bridge
        m_goBridge.SetActive(false);
        AttachedAnimal.m_bCanWalkLeft = true;
        AttachedAnimal.m_bCanWalkRight = true;

        m_headTerrain = null;
        m_headIK = null;
    }
}