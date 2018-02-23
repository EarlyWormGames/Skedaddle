using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelNotes : MonoBehaviour
{
    [System.Serializable]
    public class TextPoint
    {
        public Vector3 position;
        [Multiline] public string text;
        public bool showText = true;

        public TextPoint(Vector3 position, string text)
        {
            this.position = position;
            this.text = text;
        }
    }

    public int MaximumFontSize = 20;
    public float DistanceMultiplier = 5;
    public bool ShrinkOverDistance = true;

    public List<TextPoint> points = new List<TextPoint>();
}