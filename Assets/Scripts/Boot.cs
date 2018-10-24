using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Boot : SingleMono<Boot>
{
    public override void Awake()
    {
        AppLog.isEditor = Application.isEditor;
        AppLog.d("App.Awake 0");
        #if UNITY_EDITOR
        if(BuildConfig.Instance().UseBundle)
            BuildConfig.Instance().BundleServer.Start();
        #endif
        base.Awake();
    }

    private void Start()
    {
    }
}
