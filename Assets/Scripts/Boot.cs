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
    public override void Awake()
    {
        
        AppLog.isEditor = Application.isEditor;
        AppLog.d(Tag, "App.Awake 0");
//        #if UNITY_EDITOR
//        if(BuildConfig.Instance().UseBundle)
//            BuildConfig.Instance().BundleServer.StartBtn();
//        #endif
        base.Awake();
    }

    public override IEnumerator Init()
    {

        var conf = Application.streamingAssetsPath + "/config.lua";
        string luas = "";
        #if UNITY_ANDROID //&& !UNITY_EDITOR 
        // var www0 = new WWW(conf);
        var www = UnityEngine.Networking.UnityWebRequest.Get(conf);
        yield return www.SendWebRequest();
        luas = www.downloadHandler.text;
        #else
        if (File.Exists(conf))
        {
            luas = File.ReadAllText(conf);
        }
        #endif
        AppLog.d(Tag, "[{0}]", luas);
        LuaSys.Instance.GlobalEnv.DoString(luas);

        
        while(!AssetSys.Instance.Inited)
            yield return null;
        while(!LuaSys.Instance.Inited)
            yield return null;

        yield return AssetSys.Instance.GetBundle("lua/utility.bd");

        
        yield return AssetSys.Instance.GetAsset<TextAsset>("ui/boot/boot.lua", asset =>
        {
            var lua = gameObject.AddComponent<LuaMonoBehaviour>();
            lua.SetLua(asset);
            lua.enabled = true;
        });
        
        yield return base.Init();
    }

    private void Update()
    {
        
    }
}
