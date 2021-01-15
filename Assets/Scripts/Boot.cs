using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Video;

public class Boot : SingleMono<Boot>
{
    const string Tag = "Boot";

    public LuaMonoBehaviour bootLua;
    
    public override void Awake()
    {
        
        AppLog.isEditor = Application.isEditor;
        AppLog.d(Tag, "App.Awake 0");
        // #if UNITY_EDITOR
        // if(BuildConfig.Instance().UseBundle)
        //     BuildConfig.Instance().BundleServer.StartBtn();
        // #endif
        StartCoroutine(Init());
        base.Awake();
    }

    public override IEnumerator Init()
    {
        string luas = AssetSys.GetStreamingAsset("config.lua") as string;
        LuaSys.Instance.DoString(luas, "config");


        yield return AssetSys.Instance.Init();

        yield return LuaSys.Instance.Init() ;

        
        yield return AssetSys.Instance.GetAsset<TextAsset>("lua/utility/xlua/util.lua");

        
        // yield return AssetSys.Instance.GetAsset<TextAsset>("ui/boot/boot.lua", asset => {
        //     var lua = gameObject.AddComponent<LuaMonoBehaviour>();
        //     lua.LuaPath = "ui/boot/boot.lua";
        //     lua.SetLua(bootLua);
        //     lua.enabled = true;
        // });
        bootLua.enabled = true;
        
        yield return base.Init();
    }

    private void Update()
    {
        
    }
}
