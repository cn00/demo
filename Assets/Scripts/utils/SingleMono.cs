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
        // StartCoroutine(Init()); // 手动管理启动顺序
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
        Debug.Log($"{typeof(T).ToString()} inited.");
        yield return null;
    }
}