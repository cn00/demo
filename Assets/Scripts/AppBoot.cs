using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AppBoot : SingletonMB<AppBoot>
{

    // Use this for initialization
    public override IEnumerator Init()
    {
        yield return LuaSingleton.Instance.Init();

        GameObject obj = null;
        yield return AssetHelper.Instance.GetAsset("ui/login/login.prefab", (asset => {
           obj = asset as GameObject;
        }));
        AppLog.d("AppBoot.Start {0}", obj);
        var ui = GameObject.Instantiate(obj);

        yield return base.Init();
    }

    private void Awake()
    {
        StartCoroutine(Init());
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
        }
        catch(Exception e)
        {
            AppLog.e(e);
        }
    }
}
