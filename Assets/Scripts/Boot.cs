using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Boot : SingleMono<Boot>
{
    // Use this for initialization
    public override IEnumerator Init()
    {
        AppLog.d("App.Init 0 AssetSys");
        yield return AssetSys.Instance.Init();

        AppLog.d("App.Init 1 LuaSys");
        yield return LuaSys.Instance.Init();

        AppLog.d("App.Init 1 boot");
        var uiluamono = gameObject.AddComponent<LuaMonoBehaviour>();
        uiluamono.SetLua("ui/boot/boot");
        uiluamono.enabled = true;
    }

    private void Awake()
    {
        AppLog.isEditor = Application.isEditor;
        AppLog.d("App.Awake 0");
        #if UNITY_EDITOR
        if(BuildConfig.Instance().UseBundle)
            BuildConfig.Instance().BundleServer.Start();
        #endif
        StartCoroutine(Init());
    }

    private void Start()
    {
    }
}
