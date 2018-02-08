using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TutItems
{
    public GameObject gameObject;
    public Object type;
    public string[] names;
    public TutItemObject[] values;

    public int typeIndex;
}

[System.Serializable]
public class TutItemObject
{
    [SerializeField] public string type;
    [SerializeField] public float f;
    [SerializeField] public bool b;
    [SerializeField] public int i;
    [SerializeField] public string s;
    [SerializeField] public Vector2 v2;
    [SerializeField] public Vector3 v3;
    [SerializeField] public Object o;
}

public class TutBox : MonoBehaviour
{
    public string m_sGPText;
    public string m_sKeyText;

    public TutItems[] m_atItems;

    public bool m_bAnimalOnly = true;

    void OnTriggerEnter(Collider col)
    {
        if (m_bAnimalOnly)
        {
            if (col.gameObject.layer != LayerMask.NameToLayer("Animal"))
                return;
        }
        Trigger();
    }

    public void Trigger()
    {
        //if (Keybinding.Instance.m_aGPadConnected[0])
        //{
        //    TutPanel.Instance.ShowText(m_sGPText);
        //}
        //else
        //{
        //TutPanel.Instance.ShowText(Controller.Connected ? m_sGPText : m_sKeyText);
        //TutPanel.Instance.ShowText(m_sKeyText);
        //}

        TutHider hider = new GameObject().AddComponent<TutHider>();
        hider.name = "TutHider";
        List<string> names = new List<string>();
        List<Object> classes = new List<Object>();
        List<object> vals = new List<object>();
        for (int i = 0; i < m_atItems.Length; ++i)
        {
            for (int j = 0; j < m_atItems[i].names.Length; ++j)
            {
                names.Add(m_atItems[i].names[j]);
                classes.Add(m_atItems[i].type);
                object obj = null;
                System.Type t = ReflectionUtils.GetType(m_atItems[i].values[j].type);
                if (t == typeof(float))
                {
                    obj = m_atItems[i].values[j].f;
                }
                else if (t == typeof(int))
                {
                    obj = m_atItems[i].values[j].i;
                }
                else if (t == typeof(bool))
                {
                    obj = m_atItems[i].values[j].b;
                }
                else if (t == typeof(string))
                {
                    obj = m_atItems[i].values[j].s;
                }
                else if (t == typeof(Vector2))
                {
                    obj = m_atItems[i].values[j].v2;
                }
                else if (t == typeof(Vector3))
                {
                    obj = m_atItems[i].values[j].v3;
                }
                else
                {
                    obj = m_atItems[i].values[j].o;
                }
                vals.Add(obj);
            }
        }
        hider.Init(names.ToArray(), vals.ToArray(), classes.ToArray());

        Destroy(gameObject);
    }
}
