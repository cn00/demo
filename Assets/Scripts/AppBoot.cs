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
        AssetBundle bd = null;
        yield return BundleSys.Instance.GetBundle("ui/login.bd", (bundle => {
           bd = bundle as AssetBundle;
        }));
        AppLog.d("AppBoot.Start {0}", bd);
        GameObject obj = bd.LoadABAsset<GameObject>("ui/login/login.prefab");
        obj.SetLuaAb(bd);
        var ui = GameObject.Instantiate(obj);

        yield return base.Init();
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
