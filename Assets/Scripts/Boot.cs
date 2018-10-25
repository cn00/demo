using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Boot : SingleMono<Boot>
{
    public override void Awake()
    {
        AppLog.isEditor = Application.isEditor;
        AppLog.d("App.Awake 0");
        #if UNITY_EDITOR
        if(BuildConfig.Instance().UseBundle)
            BuildConfig.Instance().BundleServer.StartBtn();
        #endif
        base.Awake();
    }

    public override IEnumerator Init()
    {
        yield return base.Init();

        while(!AssetSys.Instance.Inited)
            yield return null;
        while(!LuaSys.Instance.Inited)
            yield return null;

        var lua = gameObject.GetComponent<LuaMonoBehaviour>();
        lua.enabled = true;
    }
}
