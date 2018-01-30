using UnityEngine;
using System.Collections;

public class Trampampoline : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public Transform m_tPoint;
    public Transform m_tRightShootDir;
    public Transform m_tLeftShootDir;
    public float m_fRightShootForce;
    public float m_fLeftShootForce;
    public Animator m_aController;
    public bool m_bWaitForMove = false;
    public bool m_bBounceOnMove = true;
    public Transform m_tEnterPoint;
    public Transform m_tExitPoint;
    public float m_fBounceForce;
    public float m_fLerpSpeed = 1f;
    public float m_fWaitTime = 1f;
    public Collider m_cBufferCollider;
    public GameObject m_goAutoClimb;

    public Transform m_tChestShootDir;
    public float m_fChestShootForce = 10;

    //==================================
    //          Internal Vars
    //==================================


    //==================================
    //          Private Vars
    //================================== 
    private RigidbodyConstraints m_rCons;
    private Animal m_aAnimal;
    private bool m_bShoot = false;
    private bool m_bBeginRightShoot = false;
    private bool m_bMoving = false;
    private bool m_bBouncing = false;
    private bool m_bBeginLeftShoot = false;
    private bool m_bExiting = false;
    private float m_fWaitTimer = 0f;

    private Chest m_cChestBounced;

    private bool m_bSquashed = false;

    //Inherited functions

    protected override void OnStart()
    {
        m_fWaitTimer = m_fWaitTime;
    }

    protected override void OnUpdate()
    {
        if (m_bSquashed)
            return;

        if (m_aAnimal != null)
            m_aAnimal.m_oCurrentObject = this;
        if (m_bMoving)
        {
            m_aAnimal.m_rBody.useGravity = false;
            m_aAnimal.transform.position = Vector3.Lerp(m_aAnimal.transform.position, m_tEnterPoint.position, Time.deltaTime * m_fLerpSpeed);
            m_fWaitTimer -= Time.deltaTime;
            m_aAnimal.m_bCheckGround = false;
            m_aAnimal.m_bOnGround = true;
            m_aAnimal.m_bOnTrampoline = true;
            m_aAnimal.m_bCanWalkLeft = false;
            m_aAnimal.m_bCanWalkRight = false;
            if (m_fWaitTimer <= 0f)
            {
                m_fWaitTimer = m_fWaitTime;
                m_bMoving = false;

                if (m_bBounceOnMove)
                {
                    m_bBouncing = true;
                    m_aAnimal.m_rBody.useGravity = true;
                    m_aAnimal.m_rBody.AddForce(Vector3.up * m_fBounceForce, ForceMode.Impulse);
                    m_aAnimal.m_bCheckGround = true;
                    m_aAnimal.m_bBouncingOnTrampoline = true;
                    m_aController.SetTrigger("Bounce");
                }
                else
                {
                    m_aAnimal.m_bCheckGround = true;
                    m_bShoot = true;
                    m_aAnimal.m_rBody.velocity = Vector3.zero;
                    m_aAnimal.transform.position = m_tPoint.position;
                }
            }
        }

        if (m_bBouncing)
        {
            m_aAnimal.m_aAnimalAnimator.SetBool("Bouncing", true);
            if (Keybinding.GetKeyDown("MoveRight") || Controller.GetDpadDown(ControllerDpad.Right) || Controller.GetStickPositionDown(true, ControllerDpad.Right) || m_bEyetrackSelected)
            {
                m_aAnimal.m_bCheckGround = true;
                m_bBeginRightShoot = true;
                m_bBouncing = false;
            }
            if (Keybinding.GetKeyDown("MoveLeft") || Controller.GetDpadDown(ControllerDpad.Left) || Controller.GetStickPositionDown(true, ControllerDpad.Left))
            {
                m_aAnimal.m_bCheckGround = true;
                m_bBouncing = false;
                m_bBeginLeftShoot = true;
            }
        }
        else
        {
            if (m_aAnimal != null)
            {
                m_aAnimal.m_aAnimalAnimator.SetBool("Bouncing", false);
            }
        }

        if (m_bExiting)
        {
            m_aAnimal.transform.position = Vector3.Lerp(m_aAnimal.transform.position, m_tExitPoint.position, Time.deltaTime * m_fLerpSpeed);
            m_fWaitTimer -= Time.deltaTime;
            m_aAnimal.m_bCheckGround = false;
            m_aAnimal.m_bOnGround = true;
            m_aAnimal.m_bCanWalkLeft = false;
            m_aAnimal.m_bCanWalkRight = false;
            m_aAnimal.m_aAnimalAnimator.SetBool("Walking", true);
            if (m_fWaitTimer <= 0f)
            {
                m_fWaitTimer = m_fWaitTime;
                m_aAnimal.m_bCanWalkLeft = true;
                m_aAnimal.m_bCanWalkRight = true;
                m_aAnimal.m_bCheckGround = true;
                m_aAnimal.m_rBody.useGravity = true;
                m_aAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");
                m_bExiting = false;
                m_aAnimal.m_oCurrentObject = null;
                m_aAnimal = null;

            }
        }
    }

    public override void AnimalEnter(Animal a_animal)
    {
        if (m_aCurrentAnimal == null) return;
        if (m_aCurrentAnimal.m_oCurrentObject != null && m_aCurrentAnimal.m_oCurrentObject != this)
            return;

        if (m_bSquashed) return;

        m_aAnimal = m_aCurrentAnimal;
        m_aAnimal.m_fFallStartY = m_aAnimal.transform.position.y;

        if (m_bBouncing)
        {
            m_aAnimal.m_rBody.velocity = Vector3.zero;
            m_aAnimal.m_rBody.AddForce(Vector3.up * m_fBounceForce, ForceMode.Impulse);
            m_aController.SetTrigger("Bounce");
            return;
        }
        if (m_bBeginLeftShoot)
        {
            m_bShoot = true;
            m_aAnimal.m_bOnTrampoline = false;
            m_aAnimal.m_bBouncingOnTrampoline = false;
            m_bBeginRightShoot = false;
            return;
        }

        if (m_bBeginRightShoot)
        {
            m_bShoot = true;
            m_aAnimal.m_bOnTrampoline = false;
            m_aAnimal.m_bBouncingOnTrampoline = false;
            m_bBeginLeftShoot = false;
            return;
        }

        if (!m_bWaitForMove)
        {
            m_bShoot = true;
            m_bBeginRightShoot = true;
            m_aAnimal.m_rBody.velocity = Vector3.zero;
            m_aAnimal.transform.position = m_tPoint.position;
        }
        else
            m_bMoving = true;
    }

    protected override void ObjectEnter(Collider a_col)
    {
        if (a_col.GetComponent<Chest>())
        {
            Chest chest = a_col.GetComponent<Chest>();
            chest.GetComponent<Rigidbody>().velocity = Vector3.zero;
            chest.GetComponent<Rigidbody>().AddForce((m_tChestShootDir.position - transform.position).normalized * m_fChestShootForce, ForceMode.Impulse);
            chest.m_bRotate = true;
            chest.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollide");
            m_cChestBounced = chest;

            m_aController.SetTrigger("Launch");
        }
    }

    void FixedUpdate()
    {
        if (m_bShoot)
        {
            float animalSizeMulti;
            switch (m_aAnimal.m_eSize)
            {
                case ANIMAL_SIZE.XS:
                    animalSizeMulti = 1;
                    break;

                case ANIMAL_SIZE.S:
                    animalSizeMulti = 5;
                    break;

                default:
                    animalSizeMulti = 1;
                    break;
            }
            m_aAnimal.m_bCanWalkLeft = false;
            m_aAnimal.m_bCanWalkRight = false;
            m_aAnimal.m_fFallStartY = m_aAnimal.transform.position.y;
            m_bShoot = false;
            m_aAnimal.m_rBody.velocity = Vector3.zero;
            m_aAnimal.transform.position = m_tPoint.position;
            m_rCons = m_aAnimal.m_rBody.constraints;
            m_aAnimal.m_rBody.constraints = RigidbodyConstraints.FreezeRotation;
            m_aAnimal.m_rBody.useGravity = true;
            if (m_bBeginRightShoot)
            {
                m_aAnimal.m_rBody.AddForce((m_tRightShootDir.position - transform.position).normalized * m_fRightShootForce * animalSizeMulti, ForceMode.Impulse);
                m_bBeginRightShoot = false;
            }
            else if (m_bBeginLeftShoot)
            {
                m_aAnimal.m_rBody.AddForce((m_tLeftShootDir.position - transform.position).normalized * m_fLeftShootForce * animalSizeMulti, ForceMode.Impulse);
                m_bBeginLeftShoot = false;
            }
            m_aAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("AnimalNoCollide");

            m_aController.SetTrigger("Launch");
        }
    }

    public void End()
    {
        if (m_aAnimal == null)
        {
            if (m_cChestBounced == null)
                return;

            m_cChestBounced.gameObject.layer = LayerMask.NameToLayer("Trigger");
            NamedEvent.TriggerEvent(m_cChestBounced.LandEvent, m_cChestBounced.m_aSoundEvents);
            m_cChestBounced = null;
        }
        else
        {
            m_aAnimal.m_rBody.constraints = m_rCons;
            m_aAnimal.m_tCollider.gameObject.layer = LayerMask.NameToLayer("Animal");
            m_aAnimal.m_bCanWalkLeft = true;
            m_aAnimal.m_bCanWalkRight = true;
            m_aAnimal.m_oCurrentObject = null;
            m_aAnimal = null;
        }
    }

    protected override void AnimalExit(Animal a_animal) { }
    //public override void DoAction() { }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }

    protected override void WrongAnimalEnter(Animal a_animal)
    {
        m_bSquashed = true;
        m_cBufferCollider.enabled = false;
        m_aController.SetBool("TooHeavy", true);
        if(m_goAutoClimb != null)
        {
            m_goAutoClimb.SetActive(false);
        }
    }

    protected override void WrongAnimalExit(Animal a_animal)
    {
        m_aAnimal = null;
        m_bSquashed = false;
        m_cBufferCollider.enabled = true;
        m_aController.SetBool("TooHeavy", false);
        if (m_goAutoClimb != null)
        {
            m_goAutoClimb.SetActive(true);
        }
    }

    void OnDrawGizmos()
    {
        if (m_tRightShootDir != null && m_tPoint != null)
        {
            Gizmos.color = Color.red;
            if (m_tRightShootDir != null)
                Gizmos.DrawLine(m_tRightShootDir.position, m_tPoint.position);
            Gizmos.color = Color.green;
            if (m_tLeftShootDir != null)
                Gizmos.DrawLine(m_tLeftShootDir.position, m_tPoint.position);
            Gizmos.color = Color.blue;
            if (m_tChestShootDir != null)
                Gizmos.DrawLine(m_tChestShootDir.position, m_tPoint.position);
        }
    }
}
