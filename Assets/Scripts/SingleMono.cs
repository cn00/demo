using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMono<T> : MonoBehaviour where T: MonoBehaviour
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
    }

    public bool Inited
    {
        get;
        protected set;
    }

    /// <summary>
    /// load resources etc.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Init()
    {
        Inited = true;
        yield return null;
    }
}
