using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referencer : MonoBehaviour, IExposedPropertyTable
{
    [SerializeField]
    private List<PropertyName> _names = new List<PropertyName>();
    [SerializeField]
    private List<Object> _objects = new List<Object>();

    public void SetReferenceValue(PropertyName id, Object obj)
    {
        int index = _names.IndexOf(id);
        if (index > -1)
        {
            _objects[index] = obj;
        }
        else
        {
            _names.Add(id);
            _objects.Add(obj);
        }
    }

    public Object GetReferenceValue(PropertyName id, out bool idValid)
    {
        int index = _names.IndexOf(id);
        if (index > -1)
        {
            idValid = true;
            return _objects[index];
        }

        idValid = false;
        return null;
    }

    public void ClearReferenceValue(PropertyName id)
    {
        int index = _names.IndexOf(id);
        if (index > -1)
        {
            _names.RemoveAt(index);
            _objects.RemoveAt(index);
        }
    }

    public bool HasReference(Object obj, out PropertyName name)
    {
        name = null;
        int index = _objects.IndexOf(obj);

        if (index > -1)
            name = _names[index];

        return index > -1;
    }
}