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

        yield return AssetSys.Instance.Init();

        yield return base.Init();

        gameObject.GetComponent<LuaMonoBehaviour>().enabled = true;
    }

    private void Awake()
    {
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
