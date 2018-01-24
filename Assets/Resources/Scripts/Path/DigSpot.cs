using UnityEngine;
using System.Collections;

public class DigSpot : SplineFollower
{
    public SkinnedMeshRenderer m_bsDigBlends;
    public SkinnedMeshRenderer m_bsDigOutBlends;
    public ParticleSystem m_psDigParticles;
    public ParticleSystem m_psDirtFallingParticles;
    public ParticleSystem m_psDirtExplosion;
    public Collider m_cGroundCollider;
    public Transform m_tCameraMove;
    public Transform m_tCameraFocus;
    public float m_fLookTime = 1f;

    private bool m_bFirst = true;
    private bool m_bStartDig;
    private float m_timer;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (m_Spline.m_MoveObject != null)
        {
            if (m_bStartDig)
            {
                m_timer = 0;
            }
            m_bStartDig = false;
            m_psDigParticles.Stop();
            m_timer += Time.deltaTime;
            if(m_bsDigOutBlends != null) m_bsDigOutBlends.SetBlendShapeWeight(1, m_timer / m_Spline.m_FollowTime * 100);

            if (m_bFirst)
            {
                if (m_psDirtFallingParticles != null)
                    m_psDirtFallingParticles.Play();
                if (m_tCameraMove != null)
                {
                    //CameraController.Instance.ViewObject(m_tCameraMove.gameObject, m_Spline.m_FollowTime, m_fLookTime, m_tCameraFocus);

                }
                m_bFirst = false;
            }
        }

        if (m_bStartDig)
        {
            m_timer += Time.deltaTime;
            if(m_timer > 0.2f && !m_psDigParticles.isPlaying) m_psDigParticles.Play();
            m_bsDigBlends.SetBlendShapeWeight(0, m_aAnimal.m_aAnimalAnimator.GetFloat("Root_Curve_Y") * 100);
        }
    }

    public override void AnimalEnter(Animal a_animal)
    {
        a_animal.m_eExtras.m_sfDigPath = this;
    }

    protected override void StartAnimation()
    {
        m_aCurrentAnimal.m_aAnimalAnimator.SetBool("Dig", true);
        m_bStartDig = true;
        m_timer = 0;
    }

    protected override void OnFinish()
    {

        m_aAnimal.m_aAnimalAnimator.SetBool("Dig", false);
        if (m_bsDigOutBlends != null) m_bsDigOutBlends.SetBlendShapeWeight(2, 100);
        if (m_cGroundCollider != null)
        {
            Destroy(m_cGroundCollider);
            m_cGroundCollider = null;
        }

        if (m_psDirtFallingParticles != null)
            m_psDirtFallingParticles.Stop();

        if (m_psDirtExplosion != null)
            m_psDirtExplosion.Play();
    }
}
