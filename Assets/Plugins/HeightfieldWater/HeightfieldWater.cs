using UnityEngine;
using System;

[Serializable]
public class WaterSettings
{
    public int m_Width = 10;
    public int m_Depth = 10;
}

public class HeightfieldWater : MonoBehaviour
{
    public float m_StartingHeight = 0.1f;
    public int m_Width = 10;
    public int m_Depth = 10;
    public float m_WaveSpeed = 0.1f;
    public float m_ColumnWidth = 1;
    public float m_ExtraForce = 1f;
    public float m_ScaleX = 10;
    public float m_ScaleY = 10;

    public float m_Damping = 0.1f;
    public float m_MinHeight = 1;
    public float m_MaxHeight = 1;

    public Material m_WaterMaterial;

    public WaterSettings[] m_GraphicsLevels = new WaterSettings[0];

    private float[,] m_Velocities;
    private float[,] m_Heights;

    private MeshFilter m_Filter;
    private MeshRenderer m_Renderer;
    private MeshCollider m_MeshCollider;

    private int m_SetWidth = 0;
    private int m_SetDepth = 0;

    private int m_GraphicsIndex = 0;
    private Texture2D motionData;

    private Material m_MatInstance;

    // Use this for initialization
    public void Awake()
    {
        m_GraphicsIndex = PlayerPrefs.GetInt("water_level", m_GraphicsLevels.Length - 1);
        GenerateMesh();

        if (GetComponent<RealtimeReflection.PlanarRealtimeReflection>())
            GetComponent<RealtimeReflection.PlanarRealtimeReflection>().m_MatInstance = m_MatInstance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            int w = m_Width;
            m_GraphicsIndex = Mathf.Clamp(m_GraphicsIndex - 1, 0, m_GraphicsLevels.Length - 1);
            m_Width = m_GraphicsLevels[m_GraphicsIndex].m_Width;
            m_Depth = m_GraphicsLevels[m_GraphicsIndex].m_Depth;

            if (w != m_Width)
                GenerateMesh();
            PlayerPrefs.SetInt("water_level", m_GraphicsIndex);
            PlayerPrefs.Save();
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            int w = m_Width;
            m_GraphicsIndex = Mathf.Clamp(m_GraphicsIndex + 1, 0, m_GraphicsLevels.Length - 1);
            m_Width = m_GraphicsLevels[m_GraphicsIndex].m_Width;
            m_Depth = m_GraphicsLevels[m_GraphicsIndex].m_Depth;

            if (w != m_Width)
                GenerateMesh();
            PlayerPrefs.SetInt("water_level", m_GraphicsIndex);
            PlayerPrefs.Save();
        }

        float speed2 = m_WaveSpeed * m_WaveSpeed;
        float h2 = m_ColumnWidth * m_ColumnWidth;
        float[,] heightOld = (float[,])m_Heights.Clone();

        for (int i = 0; i < m_SetWidth; ++i)
        {
            for (int j = 0; j < m_SetDepth; ++j)
            {
                //===============
                // Calculate new velocity
                float f = speed2 * ((i < m_SetWidth - 1 ? heightOld[i + 1, j] : 0) +
                    (i > 0 ? heightOld[i - 1, j] : 0) +
                    (j < m_SetDepth - 1 ? heightOld[i, j + 1] : 0) +
                    (j > 0 ? heightOld[i, j - 1] : 0) -
                    (4 * heightOld[i, j])) / h2;

                m_Velocities[i, j] += f * Time.fixedDeltaTime;
                m_Velocities[i, j] *= m_Damping;

                //Apply that new velocity to get new height
                float height = m_Heights[i, j] + m_Velocities[i, j] * Time.fixedDeltaTime * m_ExtraForce;

                //Clamp new height and apply it
                m_Heights[i, j] = Mathf.Clamp(height, m_MinHeight, m_MaxHeight);
                if (m_Heights[i, j] == m_MinHeight || m_Heights[i, j] == m_MaxHeight)
                    m_Velocities[i, j] = 0;

                //Calculate color based on height
                float col = 0.5f + (m_Heights[i, j] * 0.5f);

                motionData.SetPixel(i, j, new Color(col, 0, 0));
            }
        }

        motionData.Apply();

        m_MatInstance.SetTexture("_MotionData", motionData);

        //Vector3[] verts = m_Filter.mesh.vertices;
        //int index = 0;
        //for (int j = 0; j < m_SetDepth; ++j)
        //{
        //    for (int i = 0; i < m_SetWidth; ++i)
        //    {
        //        verts[index++].y = m_Heights[i, j];
        //    }
        //}
        //m_Filter.mesh.vertices = verts;
        //m_Filter.mesh.RecalculateNormals();
    }

    public void SetReflection(Texture a_tex)
    {
        m_MatInstance.SetTexture("_ScreenTex", a_tex);
    }

    public void AddForce(Vector3 a_pos, float a_force)
    {
        Vector3 dir = a_pos - transform.position;

        int x = (int)(dir.x / (m_ScaleX / m_SetWidth));
        int z = (int)(dir.z / (m_ScaleY / m_SetDepth));
        
        if (x < m_SetWidth && z < m_SetDepth && x > 0 && z > 0)
            m_Velocities[x, z] -= a_force;
    }

    void OnTriggerEnter(Collider a_col)
    {
        if (a_col.GetComponent<WaterStep>())
            a_col.GetComponent<WaterStep>().m_Water = this;
        else if (a_col.GetComponentInChildren<WaterStep>())
            a_col.GetComponentInChildren<WaterStep>().m_Water = this;
        else if (a_col.transform.parent != null)
        {
            if (a_col.GetComponentInParent<WaterStep>())
                a_col.GetComponentInParent<WaterStep>().m_Water = this;
            else if (a_col.transform.parent.GetComponentInChildren<WaterStep>())
                a_col.transform.parent.GetComponentInChildren<WaterStep>().m_Water = this;
        }
    }

    void OnTriggerExit(Collider a_col)
    {
        if (a_col.GetComponent<WaterStep>())
        {
            if (a_col.GetComponent<WaterStep>().m_Water == this)
                a_col.GetComponent<WaterStep>().m_Water = null;
        }
        else if (a_col.GetComponentInChildren<WaterStep>())
        {
            if (a_col.GetComponentInChildren<WaterStep>().m_Water == this)
                a_col.GetComponentInChildren<WaterStep>().m_Water = null;
        }
        else if (a_col.transform.parent != null)
        {
            if (a_col.GetComponentInParent<WaterStep>())
            {
                if (a_col.GetComponentInParent<WaterStep>().m_Water == this)
                    a_col.GetComponentInParent<WaterStep>().m_Water = null;
            }
            else if (a_col.transform.parent.GetComponentInChildren<WaterStep>())
            {
                if (a_col.transform.parent.GetComponentInChildren<WaterStep>().m_Water == this)
                    a_col.transform.parent.GetComponentInChildren<WaterStep>().m_Water = null;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        //if (m_Grid == null)
        //    return;
        //
        //for (int i = 0; i < size; ++i)
        //{
        //    for (int j = 0; j < size; ++j)
        //    {
        //        Vector3 pos = new Vector3(i, m_Grid[i, j].height, j);
        //        Gizmos.DrawIcon(transform.TransformPoint(pos), "black.png", false);
        //    }
        //}
    }

    public void GenerateMesh()
    {
        m_SetWidth = m_Width;
        m_SetDepth = m_Depth;
        
        m_Velocities = new float[m_Width, m_Depth];
        m_Heights = new float[m_Width, m_Depth];

        for (int i = 0; i < m_Width; ++i)
        {
            for (int j = 0; j < m_Depth; ++j)
            {
                m_Velocities[i, j] = m_StartingHeight;
                m_Velocities[i, j] = 0;
            }
        }

        int hCount2 = m_Width;
        int vCount2 = m_Depth;
        int numVertices = hCount2 * vCount2;
        int numTris = m_Width * m_Depth * 6;

        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[numTris];
        Vector4[] tangents = new Vector4[numVertices];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);


        int index = 0;
        float uvFactorX = 1.0f / m_Width;
        float uvFactorY = 1.0f / m_Depth;
        float width = m_ScaleX;
        float height = m_ScaleY;
        float scaleX = width / m_Width;
        float scaleY = height / m_Depth;

        for (float y = 0.0f; y < vCount2; y++)
        {
            for (float x = 0.0f; x < hCount2; x++)
            {
                vertices[index] = new Vector3(x * scaleX, 0.0f, y * scaleY);
                tangents[index] = tangent;
                uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
            }
        }

        index = 0;
        for (int y = 0; y < m_Depth - 1; y++)
        {
            for (int x = 0; x < m_Width - 1; x++)
            {
                triangles[index] = (y * m_Width) + x;
                triangles[index + 1] = ((y + 1) * m_Width) + x;
                triangles[index + 2] = (y * m_Width) + x + 1;

                triangles[index + 3] = ((y + 1) * m_Width) + x;
                triangles[index + 4] = ((y + 1) * m_Width) + x + 1;
                triangles[index + 5] = (y * m_Width) + x + 1;
                index += 6;
            }
        }

        m_Filter = GetComponent<MeshFilter>();
        if (m_Filter == null)
            m_Filter = gameObject.AddComponent<MeshFilter>();

        m_Renderer = GetComponent<MeshRenderer>();
        if (m_Renderer == null)
            m_Renderer = gameObject.AddComponent<MeshRenderer>();

        m_Renderer.sharedMaterial = m_WaterMaterial;
        m_MatInstance = m_Renderer.material;

        Mesh m;
        if (m_Filter.mesh != null)
        {
            m_Filter.mesh.Clear();
            m = m_Filter.mesh;
        }
        else
            m = new Mesh();

        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = triangles;
        m.tangents = tangents;
        m.RecalculateNormals();
        m.name = "Water Mesh";

        m_Filter.mesh = m;
        m.RecalculateBounds();

        BoxCollider box = gameObject.GetComponent<BoxCollider>();
        if (box == null)
            box = gameObject.AddComponent<BoxCollider>();
        else
        {
            DestroyImmediate(gameObject.GetComponent<BoxCollider>());
            box = gameObject.AddComponent<BoxCollider>();
        }

        box.isTrigger = true;
        box.size = new Vector3(box.size.x, box.size.y + 0.5f, box.size.z);
        box.center = new Vector3(box.center.x, box.center.y - 0.25f, box.center.z);
        motionData = new Texture2D(m_SetWidth, m_SetDepth, TextureFormat.RGB24, false);
    }

    //private void OnDestroy()
    //{
    //    m_WaterMaterial.SetTexture("_MotionData", null);
    //}
}
