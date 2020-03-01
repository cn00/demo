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
        AppLog.d(Tag, "AssetSys.init");
        while(!AssetSys.Instance.Inited)
            yield return null;
        AppLog.d(Tag, "LuaSys.init");
        while(!LuaSys.Instance.Inited)
            yield return null;


        {
            var luas = "";
            string path = Application.streamingAssetsPath + "/config.lua";
            #if UNITY_ANDROID
            UnityWebRequest www = new UnityWebRequest(path);

            yield return www.SendWebRequest();
            luas = www.downloadHandler.text;
            #else
            luas = File.ReadAllText(path);
            #endif

            LuaSys.Instance.GlobalEnv.DoString(luas);
        }
        
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
