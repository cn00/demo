using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
    public static GameObject Global
    {
        get
        {
            var g = GameObject.Find("Global");
            if(g == null)
            {
                g = new GameObject("Global");
            }
            return g;
        }
    }

    static T mInstance = null;
    public static T Instance
    {
        get
        {
            if(mInstance == null)
            {
                var g = Global;
                mInstance = g.GetComponent<T>();
                if(mInstance == null)
                {
                    mInstance = g.AddComponent<T>();
                }
            }
            return mInstance;
        }
    }

    private void Awake()
    {
        Init();
    }

    public virtual bool Init()
    {
        return true;
    }
}
