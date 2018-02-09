using UnityEngine;
using System.Collections;

public class PerlinNoise
{
    public static Texture2D Generate2D(int a_width, int a_height, float xOrg, float yOrg, float scale)
    {
        Texture2D tex = new Texture2D(a_width, a_height, TextureFormat.ARGB32, false);
        Color[] colours = new Color[a_width * a_height];

        for (int y = 0; y < a_width; ++y)
        {
            for (int x = 0; x < a_width; ++x)
            {
                float xCoord = xOrg + (float)x / a_width * scale;
                float yCoord = yOrg + (float)y / a_height * scale;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                colours[y * a_width + x] = new Color(sample, sample, sample);
            }
        }
        tex.SetPixels(colours);
        tex.Apply();
        return tex;
    }
}
