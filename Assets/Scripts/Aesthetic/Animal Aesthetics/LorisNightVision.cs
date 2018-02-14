using UnityEngine;
using UnityEngine.UI;
using System;
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Night Vision")]

public class LorisNightVision : MonoBehaviour
{
    public bool NightVisionOn = false;
    public GameObject NightVisionObject;
    public float TranssionSpeed = 1.0f;

    private RawImage NV_Image;
    private Material Temp_NV_Material;



    private float NV_ActiveSensitivity = 0;
    private float NV_SensitivityMin = 2;
    private float NV_SensitivityMax = 4;

    void Start()
    {
        NV_Image = NightVisionObject.GetComponent<RawImage>();
        Temp_NV_Material = Instantiate(NV_Image.material);
        NV_Image.material = Temp_NV_Material;

        if (NightVisionOn)
            NV_ActiveSensitivity = NV_SensitivityMin;
        else
            NV_ActiveSensitivity = NV_SensitivityMax;


    }

    void Update()
    {
        if (NightVisionOn)
        {
            NV_ActiveSensitivity = NV_ActiveSensitivity - Time.deltaTime * TranssionSpeed;
        }
        else
        {
            NV_ActiveSensitivity = NV_ActiveSensitivity + Time.deltaTime * TranssionSpeed;
        }

        NV_ActiveSensitivity = Mathf.Clamp(NV_ActiveSensitivity, NV_SensitivityMin, NV_SensitivityMax);

        Temp_NV_Material.SetFloat("_LightSensitivityMultiplier", NV_ActiveSensitivity);
    }

}