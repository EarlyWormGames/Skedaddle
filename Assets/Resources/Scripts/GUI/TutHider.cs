using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

public class TutHider : MonoBehaviour
{
    private Dictionary<object, MemberInfo> m_dMembers;
    private object[] m_aVals;

    public void Init(string[] a_vars, object[] a_vals, object[] a_classes)
    {
        m_aVals = a_vals;
        m_dMembers = new Dictionary<object, MemberInfo>();
        for (int i = 0; i < a_vars.Length; ++i)
        {
            Type t = a_classes[i].GetType();
            MemberInfo[] infos = t.GetMember(a_vars[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (infos != null ? infos.Length > 0 : false)
            {
                m_dMembers.Add(a_classes[i], infos[0]);
            }
            else
            {
                Debug.LogError("Supplied object does not exist in supplied class");
                Destroy(gameObject);
                return;
            }
        }
    }

    void Update()
    {
        bool hide = false;
        int i = 0;
        foreach (var pair in m_dMembers)
        {
            switch (pair.Value.MemberType)
            {
                case MemberTypes.Field:
                    {
                        object obj = ((FieldInfo)pair.Value).GetValue(pair.Key);

                        if (obj == null)
                        {
                            hide = false;
                            break;
                        }

                        hide = obj.Equals(m_aVals[i]) ? true : false;
                        break;
                    }
                case MemberTypes.Property:
                    {
                        object obj = ((PropertyInfo)pair.Value).GetValue(pair.Key, null);

                        if (obj == null)
                        {
                            hide = false;
                            break;
                        }

                        hide = obj.Equals(m_aVals[i]) ? true : false;
                        break;
                    }
                default:
                    Debug.LogError("Input MemberInfo must be of type FieldInfo or PropertyInfo");
                    break;
            }
            ++i;
        }

        if (hide)
        {
            TutPanel.Instance.Hide();
            Destroy(gameObject);
        }
    }
}
