using UnityEngine;
using System.Collections;

public class BreakableObject : ActionObject
{
    //==================================
    //          Public Vars
    //==================================
    public GameObject m_goObject;
    public ParticleSystem m_psExplosion;
    public float m_fShardForce;

    //==================================
    //          Internal Vars
    //==================================

    //==================================
    //          Private Vars
    //================================== 
    private Rigidbody[] m_rShards;

    //Inherited functions

    protected override void OnStart()
    {
        m_aRequiredAnimal = ANIMAL_NAME.ZEBRA;
        if (m_goObject != null)
        {
            m_rShards = m_goObject.GetComponentsInChildren<Rigidbody>();
        }
    }
    protected override void OnCanTrigger() { }
    //protected override void AnimalEnter(Animal a_animal) { }
    //protected override void AnimalExit(Animal a_animal) { }
    public override void DoAction()
    {
        base.DoAction();

        //This object should have the box collider on it
        foreach (Rigidbody x in m_rShards)
        {
            x.isKinematic = false;
            x.useGravity = true;
            x.GetComponent<Transform>().parent = null;
            x.GetComponent<Collider>().enabled = true;
            x.AddForce(Vector3.left * m_fShardForce, ForceMode.Impulse);
            x.AddForce(Vector3.forward * m_fShardForce, ForceMode.Impulse);
            x.Sleep();
        }
        m_psExplosion.Play();
        Destroy(gameObject);
    }
    //public override void DoActionOn() { }
    //public override void DoActionOff() { }
}
