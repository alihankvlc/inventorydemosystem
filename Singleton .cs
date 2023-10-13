using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindAnyObjectByType<T>();

                if (m_instance == null)
                    m_instance = new GameObject("Singelton_").AddComponent<T>();
            }
            return m_instance;
        }
    }
}

