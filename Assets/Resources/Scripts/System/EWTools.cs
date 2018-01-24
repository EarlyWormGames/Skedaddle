using UnityEngine;
using System.Collections;

public class EWTools
{
    public static Texture2D MakeTexture(int a_width, int a_height, Color a_col, TextureFormat a_format = TextureFormat.ARGB32, bool a_mipmap = false)
    {
        Texture2D tex = new Texture2D(a_width, a_height, a_format, a_mipmap);
        Color[] pixels = tex.GetPixels();
        for (int i = 0; i < pixels.Length; ++i)
        {
            pixels[i] = a_col;
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public static T[] AlterArray<T>(T[] a_array, int a_newLength)
    {
        T[] altArr = new T[a_newLength];

        for (int i = 0; i < altArr.Length; ++i)
        {
            if (i < a_array.Length)
            {
                altArr[i] = a_array[i];
            }
        }

        return altArr;
    }
}
