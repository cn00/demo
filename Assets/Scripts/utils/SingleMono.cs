using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMono<T> : MonoBehaviour where T: MonoBehaviour
{
    static GameObject mG = null;
    public static GameObject Global
    {
        get
        {
            if(mG == null)
            {
                mG = GameObject.Find("Global") ?? new GameObject("Global");
            }
            return mG;
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

    public virtual void Awake()
    {
        enabled = false;
        Inited = false;
        StartCoroutine(Init());
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
        enabled = true;
        AppLog.d("SingleMono", "[{0}] inited.", typeof(T));
        yield return null;
    }
}
