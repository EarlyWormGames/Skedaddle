using UnityEngine;
using UnityEngine.UI;
using System;


/// <summary>  Night Vision (NV)
/// Will use the default settings from the NV_Pannle on the NV_Canvas Prefab ( ( ( IF ) ) ) the NV_Canvas doesn't exsist in the scene
/// 
/// Change the Luminocity setting on the NV_Pannle material to increase the visibility of the Night Vision
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Night Vision")]
public class NightVision : MonoBehaviour
{
    public enum EStatus
    {
        eFADEIN,
        eFADEOUT,
        eIDLE,
    };

    public GameObject NVCanvas;
    private GameObject Temp_NV_Canvas;

    public bool NightVisionOn = false;
    public GameObject NightVisionObject;
    public float NVTranssionSpeed = 1.0f;

    public Light[] ActiveLights;
    private float[] ActiveLightIntencity;
    public float LightTransitionSpeed = 0.2f;

    private bool PreviousNVStatus;
    private RawImage NV_Image;
    private Material Temp_NV_Material;

    private EStatus m_eCurrentStatus;
    private EStatus m_ePreviousStatus;
    private bool m_bBeginNV = false;

    private Fading m_Fade;

    private float NV_ActiveSensitivity = 0;
    private float NV_SensitivityMin = 2;
    private float NV_SensitivityMax = 4;
    private bool DoLightBlend = false;
    void Start()
    {

    }

    void Awake()
    {
        GameObject NV_GO = GameObject.FindGameObjectWithTag("NVCanvas");
        if (NV_GO == null)
        {
            Temp_NV_Canvas = Instantiate(NVCanvas);
            NightVisionObject = Temp_NV_Canvas.transform.GetChild(0).gameObject;
        }
        else
        {
            NightVisionObject = NV_GO.transform.GetChild(0).gameObject;
        }
        
        
        m_eCurrentStatus = EStatus.eIDLE;
        m_ePreviousStatus = m_eCurrentStatus;   //simulating as if the scene had just faded out.

        PreviousNVStatus = NightVisionOn;
        
        m_Fade = GameManager.Instance.GetComponent<Fading>();

        NV_Image = NightVisionObject.GetComponent<RawImage>();
        Temp_NV_Material = Instantiate(NV_Image.material);
        Temp_NV_Material.name = "TEMP NV MATERIAL";
        NV_Image.material = Temp_NV_Material;

        NightVisionObject.SetActive(false);

        if (NightVisionOn)
            NV_ActiveSensitivity = NV_SensitivityMin;
        else
            NV_ActiveSensitivity = NV_SensitivityMax;

        if (ActiveLights.Length != 0)
        {
            ActiveLightIntencity = new float[ActiveLights.Length];
            for (int i = 0; i < ActiveLights.Length; i++)
            {
                if(ActiveLights[i] != null)
                ActiveLightIntencity[i] = ActiveLights[i].intensity;
            }
        }

    }

    private void OnDestroy()
    {
        DestroyImmediate(Temp_NV_Material);
        DestroyImmediate(Temp_NV_Canvas);
        
    }
    void Update()
    {
        if (PreviousNVStatus != NightVisionOn)
        {
            PreviousNVStatus = NightVisionOn;

            if (NightVisionOn)
            {
                m_Fade.BeginFadeInOut();
                RemoveListner(EndNV);
                AddListner(StartNV);
                LightsOFF();
            }
            else
            {
                m_Fade.BeginFadeInOut();
                RemoveListner(StartNV);
                AddListner(EndNV);

            }
        }

        if (NightVisionOn)
        {
            DoLightBlend = true;
        }
        else
        {
          
            var TLoris = AnimalController.Instance.GetAnimal(ANIMAL_NAME.LORIS);
            if (TLoris != null)
            {
                //I fixed it for you :YEET:
                if (!TLoris.GetComponent<Loris>().GetLightStatus()) //// THIS IS A TERRIBLE LINE OF CODE :Bernard: *was
                {
                    DoLightBlend = false;
                }
            }
        }

        if (DoLightBlend)
        {
            LightsOFF();
        }
        else
        {
            LightsON();
        }

        if (m_bBeginNV)
        {
            if (m_eCurrentStatus == EStatus.eIDLE)
            {
                if (NightVisionOn)
                {
                    if (NightVisionObject.activeSelf == false)
                        NightVisionObject.SetActive(true);

                    m_ePreviousStatus = m_eCurrentStatus;
                    m_eCurrentStatus = EStatus.eFADEIN;
                }
                else
                {
                    if (m_ePreviousStatus == EStatus.eFADEOUT)
                    {
                        NightVisionObject.SetActive(false);
                    }
                    m_ePreviousStatus = m_eCurrentStatus;
                    m_eCurrentStatus = EStatus.eFADEOUT;
                }
            }


            //Current transition of the fade between states
            switch (m_eCurrentStatus)
            {
                case EStatus.eIDLE:
                    if (m_ePreviousStatus == EStatus.eFADEOUT)
                    {
                        NightVisionObject.SetActive(false);
                    }
                    break;

                case EStatus.eFADEIN:

                    //Honestly I just dont know why I'd write all this bullshit

                    //if (m_bBeginGreenFadeIn)
                    //{
                    //    NV_ActiveSensitivity = NV_ActiveSensitivity - Time.deltaTime * TranssionSpeed;

                    //    if (NV_ActiveSensitivity <= NV_SensitivityMin)
                    //    {
                    //        m_ePreviousStatus = m_eCurrentStatus;
                    //        m_eCurrentStatus = EStatus.eIDLE;
                    //    }
                    //}
                    break;

                case EStatus.eFADEOUT:

                    //NV_ActiveSensitivity = NV_ActiveSensitivity + Time.deltaTime * TranssionSpeed;

                    //if (NV_ActiveSensitivity >= NV_SensitivityMax)
                    //{
                    //    m_ePreviousStatus = m_eCurrentStatus;
                    //    m_eCurrentStatus = EStatus.eIDLE;
                    //}
                    break;

                default:
                    Debug.LogError("Fade Status is broken, Who know's why?");
                    break;
            }
        }
        NV_ActiveSensitivity = Mathf.Clamp(NV_ActiveSensitivity, NV_SensitivityMin, NV_SensitivityMax);


        if (Temp_NV_Material == null)
        {
            Debug.LogWarning("The Temp_NV_Material is null." + "\n" + "It doesnt exsist until Run time");
        }
        else
        {
            Temp_NV_Material.SetFloat("_LightSensitivityMultiplier", NV_ActiveSensitivity);
        }
    }

    public void StartNV()
    {
        m_bBeginNV = true;
        NightVisionObject.SetActive(true);
    }

    public void EndNV()
    {
        m_bBeginNV = false;
        NightVisionObject.SetActive(false);
    }

    void AddListner(UnityEngine.Events.UnityAction listner)
    {
        m_Fade.EventToCall.AddListener(listner);
    }
    void RemoveListner(UnityEngine.Events.UnityAction listner)
    {
        m_Fade.EventToCall.RemoveListener(listner);
    }

    public void LightsON()
    {
        //blend intencity up
        for (int i = 0; i < ActiveLights.Length; i++)
        {
            ActiveLights[i].intensity += LightTransitionSpeed;

            if (ActiveLights[i].intensity > ActiveLightIntencity[i])
            {
                ActiveLights[i].intensity = ActiveLightIntencity[i];
            }
        }
    }
    public void LightsOFF()
    {
        //blend intencity down
        for (int i = 0; i < ActiveLights.Length; i++)
        {
            ActiveLights[i].intensity -= LightTransitionSpeed;

            if (ActiveLights[i].intensity < 0.0f)
            {
                ActiveLights[i].intensity = 0.0f;
            }
        }

    }
}