using UnityEngine;
using System.Collections;

namespace EWCinema
{
    [System.Serializable]
    public struct CinemaItem
    {
        public float keyframe;
        public string varName;
        public AnimationCurve curve;
        public Object myObject;

        public int iVal;
        public float fVal;
        public bool bVal;
        public string sVal;
        public Vector2 v2Val;
        public Vector3 v3Val;
        public Vector4 v4Val;
        public Quaternion qVal;
        public Object oVal;
    }

    [CreateAssetMenu(fileName = "CinemaObject", menuName = "Cinema Object")]
    public class CinemaObject : ScriptableObject
    {
        [HideInInspector] public CinemaItem[] m_Items = new CinemaItem[0];
    }
}