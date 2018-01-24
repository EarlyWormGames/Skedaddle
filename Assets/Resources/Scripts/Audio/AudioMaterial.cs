﻿using UnityEngine;
using System.Collections;

public class AudioMaterial : MonoBehaviour
{
    public enum eMaterial
    {
        HAY,
        PAPER,
        CHAIN,
        CROCKERY,
        CARPET,
    }

    public enum eSurface
    {
        WOOD,
        METAL,
        SHELF,
        SUITCASE,
        BOX,
        CAGE,
    }

    public eMaterial m_Material;
    public eSurface m_Surface;
    public bool m_IsSurface = false;
}
