using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;

namespace EWCinema
{
    public class CinemaEditor
    {
        [OnOpenAsset]
        public static bool OpenCinemaObject(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID).GetType() == typeof(CinemaObject))
            {
                Debug.Log("YAS");
                CinemaWindow.OpenWindow(EditorUtility.InstanceIDToObject(instanceID) as CinemaObject);
                return true;
            }
            return false; // we did not handle the open
        }
    }
}