using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
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
        {
            string luas = null;
            string path = Application.streamingAssetsPath + "/config.lua";
            AppLog.d(Tag, "config: {0}", path);
            #if UNITY_ANDROID && ! UNITY_EDITOR
            var www = new WWW(path);

            yield return www;
            
            luas = System.Text.Encoding.UTF8.GetString(www.bytes);
            www.Dispose();
            #else
            luas = File.ReadAllText(path);
            #endif
            AppLog.d(Tag, "[{0}]", luas);
            LuaSys.Instance.GlobalEnv.DoString(luas,path);
        }
        
        AppLog.d(Tag, "AssetSys.init");
        while(!AssetSys.Instance.Inited)
            yield return null;
        AppLog.d(Tag, "LuaSys.init");
        while(!LuaSys.Instance.Inited)
            yield return null;
        
        AppLog.d(Tag, "utility.init");
        if(BuildConfig.Instance().UseBundle)
            yield return AssetSys.Instance.GetBundle("lua/utility.bd");

        
        AppLog.d(Tag, "boot.lua");
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
