using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Boot : SingleMono<Boot>
{
    const string Tag = "Boot";
    public override void Awake()
    {
        
        AppLog.isEditor = Application.isEditor;
        AppLog.d(Tag, "App.Awake 0");
        #if UNITY_EDITOR
        if(BuildConfig.Instance().UseBundle)
            BuildConfig.Instance().BundleServer.StartBtn();
        #endif
        base.Awake();
    }

    public override IEnumerator Init()
    {
        while(!AssetSys.Instance.Inited)
            yield return null;
        while(!LuaSys.Instance.Inited)
            yield return null;

        if(BuildConfig.Instance().UseBundle)
            yield return AssetSys.Instance.GetBundle("lua/utility.bd");

        var lua = gameObject.AddComponent<LuaMonoBehaviour>();
        lua.SetLua("ui/boot/boot.lua");
        lua.enabled = true;
        yield return base.Init();
    }
}
