using UnityEngine;
using System.Collections;

namespace EW
{
    public class Activator : ActionObject
    {
        //==================================
        //          Public Vars
        //==================================
        public GameObject[] m_agDisables;
        public ParticleSystem[] m_apSystems;
        public bool m_bLorisLight = false;

        public bool m_bOnAtStart = true;

        public string ActivateEvent, DeactivateEvent;
        //==================================
        //          Internal Vars
        //==================================

        //==================================
        //          Private Vars
        //================================== 

        //Inherited functions

        protected override void OnStart()
        {
        }

        protected override void OnCanTrigger()
        {
            if (m_bOnAtStart)
            {
                m_bOnAtStart = false;
                NamedEvent.TriggerEvent(ActivateEvent, m_aSoundEvents);
            }
        }
        //protected override void AnimalEnter(Animal a_animal) { }
        //protected override void AnimalExit(Animal a_animal) { }
        //public override void DoAction() { }
        public override void DoActionOn()
        {
            for (int i = 0; i < m_agDisables.Length; ++i)
            {
                m_agDisables[i].SetActive(false);
            }

            for (int i = 0; i < m_apSystems.Length; ++i)
            {
                m_apSystems[i].Stop();
            }

            NamedEvent.TriggerEvent(DeactivateEvent, m_aSoundEvents);

            if (m_bLorisLight)
            {
                ((Loris)AnimalController.Instance.GetAnimal(ANIMAL_NAME.LORIS)).SetNightVision(true);
            }
        }

        public override void DoActionOff()
        {
            for (int i = 0; i < m_agDisables.Length; ++i)
            {
                m_agDisables[i].SetActive(true);
            }

            for (int i = 0; i < m_apSystems.Length; ++i)
            {
                m_apSystems[i].Play();
            }

            NamedEvent.TriggerEvent(ActivateEvent, m_aSoundEvents);

            if (m_bLorisLight)
            {
                CameraController.Instance.m_bUseNightVision = false;
                ((Loris)AnimalController.Instance.GetAnimal(ANIMAL_NAME.LORIS)).SetNightVision(false);
            }
        }
    }
}