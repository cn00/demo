using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using App;
using UnityEngine;
using UnityEngine.Video;

public class Boot : SingleMono<Boot>
{
    const string Tag = "Boot";
    
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

        yield return SdkSys.Instance.Init();
        
        // download boot res
        {
            yield return AssetSys.GetAsset("lua/utility/util.lua");
            yield return AssetSys.GetAsset("ui/loading/loading.prefab");
            yield return AssetSys.GetAsset("ui/dialog/dialog01.prefab");
            yield return AssetSys.GetAsset("common/config/config.lua");
            yield return AssetSys.GetAsset("common/root/root.prefab");
        }
        
        yield return base.Init();
    }

    void Start()
    {
        StartCoroutine(AssetSys.GetAsset<GameObject>("common/root/root.prefab", asset =>{
            var obj = GameObject.Instantiate(asset);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.eulerAngles = Vector3.zero;
       }));
    }

    private void Update(){}
}
