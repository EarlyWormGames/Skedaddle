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
        public List<int> ropes = new List<int>();
        public Color color = new Color(1, 1, 1, 0.2f);

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
            ropes = other.ropes;
            color = other.color;
        }
    }

    public int MaximumFontSize = 20;
    public float DistanceMultiplier = 5;
    public bool ShrinkOverDistance = true;
    public Color CurvesColor = Color.white;

    public List<TextPoint> points = new List<TextPoint>();

    public void RemoveItem(TextPoint item)
    {
        int index = points.IndexOf(item);

        foreach (var point in points)
        {
            for (int i = 0; i < point.ropes.Count; ++i)
            {
                if (point.ropes[i] == index)
                {
                    point.ropes.RemoveAt(i);
                    --i;
                    continue;
                }
                else if (point.ropes[i] > index)
                    --point.ropes[i];
            }
        }

        points.Remove(item);
    }
}