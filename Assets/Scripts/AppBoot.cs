using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AppBoot : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        AppLog.d("AppBoot.Start");
        try
        {
            bool ok = false;
            GameObject ui = null;
            BundleSys.Instance.GetBundle("ui/login.bd", "ui/login/login.prefab", asset => {
                ui = asset as GameObject;
                ok = true;
            });
            while(!ok)
                ;
            GameObject.Instantiate(ui);
        }
        catch (Exception e)
        {
            AppLog.e(e);
        }
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
