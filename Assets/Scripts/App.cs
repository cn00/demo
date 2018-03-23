using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class App : SingleMono<App>
{
    // Use this for initialization
    public override IEnumerator Init()
    {
        yield return LuaSys.Instance.Init();

        //yield return UpdateSys.Instance.Init();

        //gameObject.SetActive(false);
        //var luamono = gameObject.AddComponent<LuaMonoBehaviour>();
        //luamono.luaScript.path = "ui/boot/boot.lua";
        //gameObject.SetActive(false);

        //GameObject obj = null;
        //AppLog.d("AppBoot.Start {0}", obj);
        //yield return AssetSys.Instance.GetAsset<GameObject>("ui/loading/loading.prefab", (asset => {
        //   obj = asset;
        //}));
        //AppLog.d("AppBoot.Start {0}", obj);
        //var loading = GameObject.Instantiate(obj);

        //obj = null;
        //AppLog.d("AppBoot.Start {0}", obj);
        //yield return AssetSys.Instance.GetAsset<GameObject>("ui/login/login.prefab", (asset => {
        //    obj = asset;
        //}));
        //AppLog.d("AppBoot.Start {0}", obj);
        //var ui = GameObject.Instantiate(obj);
        //loading.SetActive(false);

        yield return base.Init();
    }

    private void Awake()
    {
        AppLog.d("AppBoot.Awake");
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
