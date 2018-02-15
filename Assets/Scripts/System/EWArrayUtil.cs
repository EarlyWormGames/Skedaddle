using UnityEngine;
using System.Collections;

public class EWArrayUtil
{
    public static void RemoveAt<T>(ref T[] a_array, int a_index)
    {
        T[] arr = new T[a_array.Length - 1];

        for (int i = 0; i < a_array.Length; ++i)
        {
            if (i < a_index)
            {
                arr[i] = a_array[i];
            }
            else if(i > a_index)
            {
                arr[i - 1] = a_array[i];
            }
        }

        a_array = arr;
    }

    public static void Add<T>(ref T[] a_array, T a_object)
    {
        T[] arr = new T[a_array.Length + 1];

        for (int i = 0; i < a_array.Length; ++i)
        {
            arr[i] = a_array[i];
        }
        arr[a_array.Length] = a_object;

        a_array = arr;
    }
}
