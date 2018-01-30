using UnityEngine;
using System.Collections;

public class TilingTexture : TrainEffects
{

    public bool m_bBounce;
    public Transform m_tJitterObject;
    public Vector2 m_v2Speed;
    public float m_fJitterStrength;
    public float m_fJitterSpeed;

    public Transform m_tMovePoint;
    public Transform m_tResetPoint;

    public Renderer[] m_Renderers;

    private float m_fUVTimer;
    private Terrain m_tTerrain;
    private float m_fJitter;
    private float m_fBaseHeight;
    private float m_fTimer;
    private bool m_bStartTime;
    private bool m_bStarted;

    public override void OnStart()
    {
        if (GetComponent<Terrain>() != null)
        {
            m_tTerrain = GetComponent<Terrain>();
        }
        m_fTimer = m_fMicroOffset;
        m_bStartTime = false;
        if (m_tJitterObject != null) m_fBaseHeight = m_tJitterObject.position.y;
    }

    public override void OnUpdate()
    {
        if (m_bStartTime)
        {
            m_fTimer -= Time.deltaTime;
            if (m_fTimer < 0)
            {
                if (!m_bStarted)
                {
                    m_fTimer = m_fMicroDuration;
                    m_bStarted = true;
                    m_bIsShaking = true;
                }
                else
                {
                    m_bStarted = false;
                    m_bIsShaking = false;
                    m_fTimer = m_fMicroOffset;
                    m_bStartTime = false;
                }
            }
        }
        if (m_fUVTimer < 1000)
        {
            m_fUVTimer += Time.fixedDeltaTime;
        }
        else
        {
            m_fUVTimer = 0;
        }
        if (m_tTerrain != null)
        {
            //SplatPrototype[] splatPrototypes = m_tTerrain.terrainData.splatPrototypes;
            //foreach (SplatPrototype splat in splatPrototypes)
            //{
            //    splat.tileOffset = new Vector2(m_fUVTimer * m_v2Speed.x, m_fUVTimer * m_v2Speed.y);
            //}
            //m_tTerrain.terrainData.splatPrototypes = splatPrototypes;
            m_tTerrain.transform.parent.Translate(-m_v2Speed.x * Time.deltaTime, 0, 0);

            if (m_tTerrain.transform.parent.position.x <= m_tMovePoint.position.x)
            {
                Vector3 difference = m_tTerrain.transform.parent.position - m_tMovePoint.position;
                m_tTerrain.transform.parent.position = m_tResetPoint.position - difference;
            }
        }
        if (m_Renderers.Length > 0)
        {
            foreach (var item in m_Renderers)
            {
                item.material.mainTextureOffset = new Vector2(m_fUVTimer * m_v2Speed.x, m_fUVTimer * m_v2Speed.y);
            }
        }

        if (m_tJitterObject != null)
        {
            m_tJitterObject.position = new Vector3(m_tJitterObject.position.x, m_fBaseHeight + m_fJitter, m_tJitterObject.position.z);
        }
    }

    public override void Active()
    {
        m_fJitter = m_fJitterStrength * (m_bBounce ? Mathf.Abs(Mathf.Sin(m_fJitterSpeed * m_fUVTimer)) : Mathf.Sin(m_fJitterSpeed * m_fUVTimer));
    }

    public override void Unactive()
    {
        m_fJitter = 0;
    }

    public override void OnActivation()
    {
        m_bStartTime = true;
        m_fTimer = m_fMicroOffset;
    }
}
