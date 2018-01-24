using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraScale : MonoBehaviour
{
    public int Width = 1280;
    public int Height = 720;

    public bool Scale;
    bool wasScale;

    RenderTexture targ;
    Camera cam;

    int lastW, lastH;

    private void Start()
    {
        cam = GetComponent<Camera>();
        targ = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGBFloat);
        if (Scale)
        {
            cam.targetTexture = targ;
        }

        lastW = Width;
        lastH = Height;
    }

    private void OnPreRender()
    {
        if (lastH != Height || lastW != Width)
        {
            lastW = Width;
            lastH = Height;
            targ.Release();
            targ = new RenderTexture(Width, Height, 24, RenderTextureFormat.ARGBFloat);
        }

        if(Scale)
        {
            cam.targetTexture = targ;
            wasScale = true;
        }
        else
        {
            cam.targetTexture = null;
            wasScale = false;
        }

    }

    private void OnPostRender()
    {
        if (wasScale)
        {
            cam.targetTexture = null;
            Graphics.Blit(targ, null as RenderTexture);
        }
    }

    private void OnDestroy()
    {
        targ.Release();
    }
}
