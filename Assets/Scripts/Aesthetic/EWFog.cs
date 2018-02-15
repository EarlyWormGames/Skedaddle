using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class EWFog : MonoBehaviour
{
    public Texture3D tex;
    public Texture2D other;
    [SerializeField] private bool m_bDone = false;
    // Use this for initialization
    void Awake()
    {
        CreateTex();
    }

    public void CreateTex()
    {
        if (!m_bDone)
        {
        }
    }
}
