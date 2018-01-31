using UnityEngine;
using UnityStandardAssets.CinematicEffects;
using System.Collections.Generic;

public delegate void CameraEndDelegate();

public class CameraController : Singleton<CameraController>
{
    public static CameraEndDelegate onEnd
    {
        get
        {
            return Instance.m_cEndDelegate;
        }
        set
        {
            Instance.m_cEndDelegate = value;
        }
    }

    new public Camera camera
    {
        get
        {
            if (m_cCamera == null)
                m_cCamera = GetComponent<Camera>();
            return m_cCamera;
        }
    }

    //=============
    //PUBLIC
    public Animal m_aAnimal;
    public Rigidbody m_rBody;

    public float m_fYAdd = 0f;
    public float m_fCameraSpeed = 4f;
    public float m_fRotateSpeed = 2f;
    public float m_fFocalSpeed = 2f;
    public Vector2 m_fAnimalSpeedBuffer = new Vector2 (0.5f, 0.5f);
    public bool m_bAllowManualOffsets = false;
    public bool m_bFollowAnimalZ = false;

    public AnimationCurve m_aLerpCurve;

    public float m_LightSpeed = 1f;
    public float m_VingetteSpeed = 1f;
    public float m_VingetteMax = 2;
    [HideInInspector] public bool m_ItsLitFam = false;
    //=============

    internal Vector3 m_v3Target;
    internal Transform m_tLookAt;
    internal Vector3 m_v3StartPos;
    internal float m_fStartFocus = 0f;
    internal float m_fStartSize = 0f;

    internal Vector2 m_v2XLimits;
    internal Vector2 m_v2YLimits;

    internal bool m_bUseNightVision = false;
    internal bool m_bLorisSelected = false;

    //=============
    //PRIVATE
    private Camera m_cCamera;
    private bool m_bFollow = true;
    private float m_fWaitTime = 0;
    private Quaternion m_qStartRot;
    private bool m_bCancelLook = false;
    private bool m_bReload = false;

    private Vector3 m_v3LerpStart;
    private float m_fLookTime;
    private float m_fLookTimer;
    private Vector2 m_v2SpeedOffset;
    private CameraEndDelegate m_cEndDelegate;
    private bool m_bEnded;
    private bool m_bTimeLerp = true;

    private LensAberrations m_Vingette;
    private float m_VingetteStart = 0f;

    private List<Light> m_Lights;
    private List<float> m_LightStarts;

    private Light[] m_LorisLight;

    private float m_fStartz;
    private float m_fPlayerOffsetx;
    private float m_fPlayerOffsety;
    private float m_fPlayerOffsetz;

    //=============

    // Use this for initialization
    void Start()
    {
        m_fStartz = transform.position.z;
        m_Vingette = GetComponent<LensAberrations>();
        if (m_Vingette != null)
            m_VingetteStart = m_Vingette.vignette.intensity;

        m_v3Target = transform.position;
        m_cCamera = GetComponent<Camera>();
        m_qStartRot = transform.rotation;
        m_v3StartPos = transform.position;
        //m_v2XLimits = new Vector2(transform.TransformPoint(new Vector3(m_v2XLimits.x, 0, 0)).x, transform.TransformPoint(new Vector3(m_v2XLimits.y, 0, 0)).x);


        m_Lights = new List<Light>(FindObjectsOfType<Light>());
        m_LightStarts = new List<float>();
        foreach (Light child in m_Lights)
        {
            m_LightStarts.Add(child.intensity);
        }
        AnimalController.Instance.Recheck();

        if (AnimalController.Instance.GetAnimal(ANIMAL_NAME.LORIS) != null)
            m_LorisLight = ((Loris)AnimalController.Instance.GetAnimal(ANIMAL_NAME.LORIS)).m_lVisionLight;

        if (m_LorisLight != null)
        {
            foreach (Light light in m_LorisLight)
            {
                m_LightStarts.RemoveAt(m_Lights.IndexOf(light));
                m_Lights.Remove(light);
            }
        }

        Tobii.EyeTracking.EyeTracking.SetCurrentUserViewPointCamera(camera);
    }

    void OnPreCull()
    {
        //Shader.SetGlobalMatrix("node_6897", m_cCamera.cameraToWorldMatrix);
    }

    void Update()
    {
        m_bLorisSelected = AnimalController.Instance.GetAnimalSelected(ANIMAL_NAME.LORIS);
        
        if (m_bUseNightVision)
        {
            if (m_Vingette != null)
                m_Vingette.vignette.intensity = Mathf.Lerp(m_Vingette.vignette.intensity, m_VingetteMax, Time.deltaTime * m_VingetteSpeed);
            if (m_bLorisSelected)
            {
                LetThereBeLight();
                m_ItsLitFam = true;
            }
            else
                LightsOut();
        }
        else
        {
            if (m_Vingette != null)
                m_Vingette.vignette.intensity = Mathf.Lerp(m_Vingette.vignette.intensity, m_VingetteStart, Time.deltaTime * m_VingetteSpeed);
            LetThereBeLight();
        }

        //Ray ray = m_cCamera.ScreenPointToRay(EyeTracking.position);
        //Debug.DrawLine(ray.origin, ray.direction * 100, Color.red);

        if (m_tLookAt != null)
        {
            Quaternion oldRot = transform.rotation;
            transform.LookAt(m_tLookAt);
            Quaternion rot = transform.rotation;
            transform.rotation = Quaternion.Lerp(oldRot, rot, Time.deltaTime * m_fRotateSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, m_qStartRot, Time.deltaTime * m_fRotateSpeed);
        }

        if (Keybinding.GetKey("CameraPanLeft"))
        {
            m_fPlayerOffsetx -= Time.deltaTime;
        }
        if (Keybinding.GetKey("CameraPanRight"))
        {
            m_fPlayerOffsetx += Time.deltaTime;
        }
        if (Keybinding.GetKey("CameraPanDown"))
        {
            m_fPlayerOffsety -= Time.deltaTime;
        }
        if (Keybinding.GetKey("CameraPanUp"))
        {
            m_fPlayerOffsety += Time.deltaTime;
        }
        if (Keybinding.GetKey("CameraPanOut"))
        {
            m_fPlayerOffsetz -= Time.deltaTime;
        }
        if (Keybinding.GetKey("CameraPanIn"))
        {
            m_fPlayerOffsetz += Time.deltaTime;
        }

        if (m_aAnimal != null && m_bFollow)
        {
            m_v2SpeedOffset = new Vector2(m_aAnimal.m_rBody.velocity.x * m_fAnimalSpeedBuffer.x, m_aAnimal.m_rBody.velocity.y * m_fAnimalSpeedBuffer.y);
            m_v3Target.x = m_aAnimal.m_tCameraPivot.position.x + m_v2SpeedOffset.x + (m_bAllowManualOffsets ? m_fPlayerOffsetx : 0);
            m_v3Target.y = m_aAnimal.m_tCameraPivot.position.y + m_fYAdd + m_aAnimal.m_fCameraY + m_v2SpeedOffset.y + (m_bAllowManualOffsets ? m_fPlayerOffsety : 0);
            m_v3Target.z = (m_bAllowManualOffsets ? m_fPlayerOffsetz : 0) + m_fStartz + (m_bFollowAnimalZ ? m_aAnimal.m_tCameraPivot.position.z : 0);

            if (m_v3Target.x < m_v2XLimits.x)
            {
                m_v3Target.x = m_v2XLimits.x;
            }
            else if (m_v3Target.x > m_v2XLimits.y)
            {
                m_v3Target.x = m_v2XLimits.y;
            }

            if (m_v3Target.y < m_v2YLimits.x)
            {
                m_v3Target.y = m_v2YLimits.x;
            }
            else if (m_v3Target.y > m_v2YLimits.y + m_fYAdd)
            {
                m_v3Target.y = m_v2YLimits.y + m_fYAdd;
            }
            transform.position = Vector3.Slerp(transform.position, m_v3Target, Time.deltaTime * m_fCameraSpeed);
        }
        else if (!m_bFollow)
        {
            if (m_bTimeLerp)
            {
                if (!m_bEnded)
                {
                    m_fLookTimer += Time.deltaTime;

                    float t = m_aLerpCurve.Evaluate(Mathf.Min(1, m_fLookTimer / m_fLookTime));
                    transform.position = Vector3.Lerp(m_v3LerpStart, m_v3Target, t);

                    if (t >= 1)
                    {
                        if (m_cEndDelegate != null && !m_bEnded)
                        {
                            m_cEndDelegate();
                            m_bEnded = true;
                        }
                    }
                }

                if (m_fLookTimer >= m_fWaitTime)
                {
                    if (m_bReload)
                    {
                        m_bReload = false;
                        m_fWaitTime = 10f;
                        EWApplication.ReloadLevel();
                    }
                    else
                    {
                        m_bFollow = true;

                        if (m_bCancelLook)
                        {
                            m_bCancelLook = false;
                            m_tLookAt = null;
                        }
                    }
                }
            }
            else
            {
                m_fLookTimer += Time.deltaTime;
                float t = m_aLerpCurve.Evaluate(Mathf.Min(1, m_fLookTimer / m_fLookTime));
                transform.position = Vector3.Lerp(m_v3LerpStart, m_v3Target, t);

                if (m_cEndDelegate != null && !m_bEnded)
                {
                    m_cEndDelegate();
                }

                if (m_fLookTimer >= m_fWaitTime)
                {
                    if (m_bReload)
                    {
                        m_bReload = false;
                        m_fWaitTime = 10f;
                        EWApplication.ReloadLevel();
                    }
                    else
                    {
                        m_bFollow = true;

                        if (m_bCancelLook)
                        {
                            m_bCancelLook = false;
                            m_tLookAt = null;
                        }
                    }
                }
            }
        }

    }

    public void ViewObject(GameObject a_object, float a_waitTimer = 1f, float a_lookTime = 1f, Transform a_lookAt = null, bool a_bCancelLookAt = true)
    {
        m_v3Target.x = a_object.transform.position.x;
        m_v3Target.y = a_object.transform.position.y;
        m_v3Target.z = transform.position.z;

        m_fWaitTime = a_waitTimer;
        m_fLookTime = a_lookTime;
        m_bFollow = false;
        m_tLookAt = a_lookAt;
        m_bCancelLook = a_bCancelLookAt;
        m_fLookTimer = 0f;
        m_v3LerpStart = transform.position;
        m_bEnded = false;
        m_bTimeLerp = false;
    }

    public void ViewThenReload(Vector3 a_pos, float a_waitTimer = 1f, float a_lookTime = 1f)
    {
        m_v3Target = a_pos;
        m_fWaitTime = a_waitTimer;
        m_fLookTime = a_lookTime;
        m_bFollow = false;
        m_bReload = true;
        m_v3LerpStart = transform.position;
        m_fLookTimer = 0f;
        m_bEnded = false;
        m_bTimeLerp = true;
    }

    public void GoToPoint(Vector3 a_point, float a_waitTimer = float.PositiveInfinity, float a_lookTime = 1f)
    {
        m_v3Target = a_point;
        m_fWaitTime = a_waitTimer;
        m_fLookTime = a_lookTime;
        m_bFollow = false;
        m_v3LerpStart = transform.position;
        m_fLookTimer = 0f;
        m_bEnded = false;
        m_bTimeLerp = true;
    }

    public void LerpToPoint(Vector3 a_point, float a_waitTimer = float.PositiveInfinity, float a_lookSpeed = 1f)
    {
        m_v3Target = a_point;
        m_fWaitTime = a_waitTimer;
        m_fLookTime = a_lookSpeed;
        m_bFollow = false;
        m_v3LerpStart = transform.position;
        m_fLookTimer = 0f;
        m_bEnded = false;
        m_bTimeLerp = false;
    }

    public void FollowAnimal()
    {
        m_bFollow = true;
        m_v3Target.z = m_fStartz;
    }

    public void Switch(Animal a_animal)
    {
        //if (SceneManager.GetActiveScene().name == "1-4")
        //{
        //    //TutPanel.Instance.Hide();
        //}

        //GameObject.Find("GameController").GetComponent<Audio>().ChangeAnimalMusic(a_animal.name);
        m_aAnimal = a_animal;
    }

    public void LightsOut()
    {
        foreach (Light child in m_Lights)
        {
            child.intensity = Mathf.Lerp(child.intensity, 0, Time.deltaTime * m_LightSpeed);
        }
    }

    public void LetThereBeLight()
    {
        int i = 0;
        foreach (Light child in m_Lights)
        {
            child.intensity = Mathf.Lerp(child.intensity, m_LightStarts[i], Time.deltaTime * m_LightSpeed);
            ++i;
        }
    }
}
