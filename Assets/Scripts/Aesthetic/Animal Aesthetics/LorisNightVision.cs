using UnityEngine;
using UnityEngine.UI;
using System;
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Night Vision")]

public class LorisNightVision : MonoBehaviour
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
    public float TranssionSpeed = 1.0f;

    private bool PreviousNVStatus;
    private RawImage NV_Image;
    private Material Temp_NV_Material;

    private EStatus m_eCurrentStatus;
    private EStatus m_ePreviousStatus;
    private bool m_bBeginNV = false;

    private Fading m_Fade;
    private bool m_bNVStatusJustChanged = false;

    private float NV_ActiveSensitivity = 0;
    private float NV_SensitivityMin = 2;
    private float NV_SensitivityMax = 4;

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
        AddListner(StartNV);

        NV_Image = NightVisionObject.GetComponent<RawImage>();
        Temp_NV_Material = Instantiate(NV_Image.material);
        NV_Image.material = Temp_NV_Material;

        NightVisionObject.SetActive(false);

        if (NightVisionOn)
            NV_ActiveSensitivity = NV_SensitivityMin;
        else
            NV_ActiveSensitivity = NV_SensitivityMax;
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
            m_bNVStatusJustChanged = true;

            if (NightVisionOn)
                m_Fade.BeginFadeCut(1);
            else
                m_Fade.BeginFadeCut(-1);
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
                       // EndNV();
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

                    NV_ActiveSensitivity = NV_ActiveSensitivity - Time.deltaTime * TranssionSpeed;

                    if (NV_ActiveSensitivity <= NV_SensitivityMin)
                    {
                        m_ePreviousStatus = m_eCurrentStatus;
                        m_eCurrentStatus = EStatus.eIDLE;
                    }

                    break;

                case EStatus.eFADEOUT:
                    NV_ActiveSensitivity = NV_ActiveSensitivity + Time.deltaTime * TranssionSpeed;

                    if (NV_ActiveSensitivity >= NV_SensitivityMax)
                    {
                        m_ePreviousStatus = m_eCurrentStatus;
                        m_eCurrentStatus = EStatus.eIDLE;
                    }
                    break;

                default:
                    Debug.LogError("Fade Status is broken, Who know's why?");
                    break;
            }
        }
        NV_ActiveSensitivity = Mathf.Clamp(NV_ActiveSensitivity, NV_SensitivityMin, NV_SensitivityMax);


        if (Temp_NV_Material == null)
        {
            Debug.LogWarning("The Temp_NV_Material is null." +"\n" + "It doesnt exsist until Run time");
        }
        else
        {
            Temp_NV_Material.SetFloat("_LightSensitivityMultiplier", NV_ActiveSensitivity);
        }


        m_bNVStatusJustChanged = false;
    }

    public void StartNV()
    {
        m_bBeginNV = true;
    }

    public void EndNV()
    {
        m_bBeginNV = false;
        m_Fade.FadeOut();
    }

    void AddListner(Action listner)
    {
        m_Fade.EventToCall.AddListener(StartNV);
    }
    void RemoveListner()
    {
        m_Fade.EventToCall.RemoveListener(StartNV);
    }
}