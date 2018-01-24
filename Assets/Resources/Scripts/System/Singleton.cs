using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T        m_tInstance = null;
    private static object   m_oLock = new object();

    public bool             m_bLogSingletonMessages = true;
    public bool             m_bDestroyEntireObject = false;

    void Awake()
    {
        if (m_tInstance != null)
        {
            if (m_bLogSingletonMessages)
            {
                Debug.Log("[Singleton] Destroying " + this);
            }
            (this as Singleton<T>).Dispose();
        }
        else
        {
            if (m_bLogSingletonMessages)
            {
                Debug.Log("Making new (" + typeof(T).ToString() + ")");
            }
            m_tInstance = this as T;
        }

        OnAwake();
    }

    protected void DontDestroy()
    {
        DontDestroyOnLoad(this);
    }

    public static T Instance
    {
        get
        {
            lock (m_oLock)
            {
                if (m_tInstance == null)
                {
                    m_tInstance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        if ((m_tInstance as Singleton<T>).m_bLogSingletonMessages)
                        {
                            Debug.LogError("[Singleton] There are multiple instances of (" + typeof(T).ToString() + ")! This should never happen.");
                        }
                        (m_tInstance as Singleton<T>).Reset();
                        return m_tInstance;
                    }

                    if (m_tInstance == null)
                    {
                        GameObject singleton = new GameObject();
                        m_tInstance = singleton.AddComponent<T>();
                        (m_tInstance as Singleton<T>).Reset();
                        (m_tInstance as Singleton<T>).OnCreated();
                        if ((m_tInstance as Singleton<T>).m_bLogSingletonMessages)
                        {
                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                "' was created with DontDestroyOnLoad as *" + singleton + "*");
                        }
                    }
                    else
                    {
                        if ((m_tInstance as Singleton<T>).m_bLogSingletonMessages)
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                m_tInstance.gameObject.name);
                        }
                        (m_tInstance as Singleton<T>).Reset();
                    }
                }

                return m_tInstance;
            }
        }
    }

    public static bool InstanceExists()
    {
        return m_tInstance != null;
    }

    /// <summary>
    /// Only called when it was created via calling T.Instance
    /// </summary>
    protected virtual void OnCreated() { }

    protected virtual void OnAwake() { }

    public void Dispose()
    {
        if (m_bDestroyEntireObject)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public virtual void Reset() { }
}
