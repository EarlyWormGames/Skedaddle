using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

/// <summary>
/// Edge detection of a selected object
/// 
/// Note: 
/// used primarly in the menu
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Highlighter : MonoBehaviour
{
    public enum EdgeDetectMode
    {
        TriangleDepthNormals = 0,
        RobertsCrossDepthNormals = 1,
        SobelDepth = 2,
        SobelDepthThin = 3,
        TriangleLuminance = 4,
    }

    public static GameObject Selected
    {
        get
        {
            return m_ObjRender;
        }
        set
        {
            m_ObjRender = value;
        }
    }

    //=====================================================
    //Edge detect settings
    public EdgeDetectMode mode = EdgeDetectMode.SobelDepthThin;
    public float sensitivityDepth = 1.0f;
    public float sensitivityNormals = 1.0f;
    public float lumThreshold = 0.2f;
    public float edgeExp = 1.0f;
    public float sampleDist = 1.0f;
    public float edgesOnly = 0.0f;
    public Color edgesOnlyBgColor = Color.white;
    public Shader m_EdgeDetectShader;
    //=====================================================

    //=====================================================
    //Rendering settings
    private CommandBuffer m_cb; //The command buffer to do the rendering
    private static GameObject m_ObjRender; //The object to render on post
    private Material m_UnlitMat; //Standard, unlit material
    private Material m_EdgeMat;
    new private Camera camera;
    private bool m_ParentFound = false;
    //=====================================================

    void Awake()
    {
        m_cb = new CommandBuffer();
        m_cb.ClearRenderTarget(true, false, Color.blue);

        //Get unlit shader
        m_UnlitMat = new Material(Shader.Find("Unlit/Unlit"));
        //Get edge shader
        m_EdgeMat = new Material(m_EdgeDetectShader);

        camera = GetComponent<Camera>();
        m_ObjRender = null;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_ObjRender == null)
        {
            Graphics.Blit(source, destination, m_EdgeMat, (int)mode);
            return;
        }

        m_cb.Clear(); //Clear the commands (the list of drawing we want)
        //m_Tex.Release();

        Render(m_ObjRender);
        m_ParentFound = false;

        Vector2 sensitivity = new Vector2(sensitivityDepth, sensitivityNormals);
        m_EdgeMat.SetVector("_Sensitivity", new Vector4(sensitivity.x, sensitivity.y, 1.0f, sensitivity.y));
        m_EdgeMat.SetFloat("_BgFade", edgesOnly);
        m_EdgeMat.SetFloat("_SampleDistance", sampleDist);
        m_EdgeMat.SetVector("_BgColor", edgesOnlyBgColor);
        m_EdgeMat.SetFloat("_Exponent", edgeExp);
        m_EdgeMat.SetFloat("_Threshold", lumThreshold);


        Graphics.SetRenderTarget(source);
        //GL.Clear(true, true, new Color(0, 0, 0, 0));
        GL.LoadProjectionMatrix(GL.GetGPUProjectionMatrix(camera.projectionMatrix, false));
        GL.modelview = camera.worldToCameraMatrix;

        //And now it executes the render command from the buffer we created
        Graphics.ExecuteCommandBuffer(m_cb);

        Graphics.Blit(source, destination, m_EdgeMat, (int)mode);

        m_ObjRender = null;
    }

    void Render(GameObject a_obj)
    {
        HighlightObject high = a_obj.GetComponent<HighlightObject>();
        //Add the parent and all child renderers to the list of draw commands (with a defualt unlit material to reduce processing)
        if (high != null)
        {
            if (high.IsParent)
            {
                if (m_ParentFound)
                    return;
                m_ParentFound = true;
            }

            Renderer r = a_obj.GetComponent<Renderer>();
            if (r != null)
                m_cb.DrawRenderer(r, m_UnlitMat);
        }
        foreach (Transform child in a_obj.transform)
        {
            Render(child.gameObject);
        }
    }
}
