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

    public bool NightVisionOn = false;
    public GameObject NightVisionObject;
    public float TranssionSpeed = 1.0f;

    private bool PreviousNVStatus;
    private RawImage NV_Image;
    private Material Temp_NV_Material;

    private EStatus m_eCurrentStatus;
    private EStatus m_ePreviousStatus;
    private bool m_bStatusJustChanged = false;

    private GameManager GM;
    private Fading m_Fade;

    private float NV_ActiveSensitivity = 0;
    private float NV_SensitivityMin = 2;
    private float NV_SensitivityMax = 4;

    void Start()
    {
        m_eCurrentStatus = EStatus.eIDLE;
        m_ePreviousStatus = m_eCurrentStatus;

        PreviousNVStatus = NightVisionOn;

        m_Fade = GameManager.Instance.GetComponent<Fading>();

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
    }
    void Update()
    {
        

        if (m_eCurrentStatus == EStatus.eIDLE)
        {
            if (PreviousNVStatus != NightVisionOn)
            {
                PreviousNVStatus = NightVisionOn;
                if (NightVisionOn)
                {
                    

                    if (NightVisionObject.activeSelf == false)
                        NightVisionObject.SetActive(true);
                    m_eCurrentStatus = EStatus.eFADEIN;
                }
                else
                {
                    m_eCurrentStatus = EStatus.eFADEOUT;

                }
            }
        }
       
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
        

        NV_ActiveSensitivity = Mathf.Clamp(NV_ActiveSensitivity, NV_SensitivityMin, NV_SensitivityMax);

        Temp_NV_Material.SetFloat("_LightSensitivityMultiplier", NV_ActiveSensitivity);
    }

}