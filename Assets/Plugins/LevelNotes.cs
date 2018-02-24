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
        public int joinedPoint = -1;
        public Color color = Color.white;

        public TextPoint(Vector3 position, string text)
        {
            this.position = position;
            this.text = text;
        }

        public TextPoint(TextPoint other)
        {
            position = other.position;
            text = other.text;
            showText = other.showText;
            joinedPoint = other.joinedPoint;
            color = other.color;
        }
    }

    public int MaximumFontSize = 20;
    public float DistanceMultiplier = 5;
    public bool ShrinkOverDistance = true;

    public List<TextPoint> points = new List<TextPoint>();

    public void RemoveItem(TextPoint item)
    {
        int index = points.IndexOf(item);

        foreach (var point in points)
        {
            if (point.joinedPoint == index)
                point.joinedPoint = -1;
            else if (point.joinedPoint > index)
                --point.joinedPoint;
        }

        points.Remove(item);
    }
}