using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMB<T> : MonoBehaviour where T: MonoBehaviour
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
        Inited = false;
        StartCoroutine(Init());
    }

    public bool Inited
    {
        get;
        protected set;
    }
    public virtual IEnumerator Init()
    {
        Inited = true;
        yield return null;
    }
}
