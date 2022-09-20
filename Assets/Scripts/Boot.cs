using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using App;
using Helper;
using IFix.Core;
using UnityEngine;
using UnityEngine.Video;

public class Boot : SingleMono<Boot>
{
    const string Tag = "Boot";

    private void Awake()
    {
        StartCoroutine(Init());
    }

    public static bool IFixPatched = false;
    public override IEnumerator Init()
    {
        AppLog.isEditor = Application.isEditor;
        AppLog.d(Tag, "App.Awake 0");

        yield return AssetSys.Instance.Init();

        yield return LuaSys.Instance.Init() ;

        string luas = AssetSys.GetStreamingAsset("config.lua") as string;
        LuaSys.Instance.DoString(luas, "config");

        yield return SdkSys.Instance.Init();

        // download boot res
        {
            var ifix_patch_path = $"ifix/{AssetSys.PlatformName()}/Assembly-CSharp.patch.ifix";
            yield return AssetSys.GetAsset<IFixAsset>(ifix_patch_path, asset =>
            {
                if(asset!=null)
                {
                    try
                    {
                        PatchManager.Load(new MemoryStream(asset.data));
                        IFixPatched = true;
                        StartCoroutine(IFixTest());
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError(e);
                    }
                }
                else
                {
                    Debug.LogError($"AppResourceManager.Load({ifix_patch_path}) failed.");
                }
            });

            yield return AssetSys.GetAsset("lua/utility/util.lua");
            yield return AssetSys.GetAsset("ui/loading/loading.prefab");
            yield return AssetSys.GetAsset("ui/dialog/dialog01.prefab");
            yield return AssetSys.GetAsset("common/config/config.lua");
            yield return AssetSys.GetAsset("common/root/root.prefab");
        }

        yield return AssetSys.GetAsset<GameObject>("common/root/root.prefab", asset =>
        {
            var obj = GameObject.Instantiate(asset);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.eulerAngles = Vector3.zero;
        });

        yield return base.Init();
    }

    public delegate void Action6(int i1, int i2, int i3, float f1, float f2, float f3);

    [IFix.Patch]
    public static IEnumerator IFixTest()
    {
#if UNITY_EDITORs // && USE_LOG // 上线请加上 USE_LOG
        Debug.LogError("IFix 别慌，我是热更代码测试 begin");
        IFixNewMethodTest(32, "IFix string");
        Action<int> ddddd = (i1) => // , i2, i3, f1, f2, f3
        {
            Debug.LogError("IFix 别慌，我是 Patch 代码匿名函数 ddddd");
        };
        ddddd(1); //,2,3, 1f,2f,3f
        Debug.LogError("IFix 别慌，我是热更代码测试 end");
#endif
        yield return null;
    }

#if UNITY_EDITOR //patch 后 UNITY_EDITOR 宏内的代码会在手机端执行
    [IFix.Interpret]
    public static float iFixNewFieldTest = 3.1415926f;

    [IFix.Interpret]
    public static int iFixNewPropertyTest
    {
        get{return 10086;}
    }

    [IFix.Interpret]
    public static void IFixNewMethodTest(int  i, string s)
    {
        Debug.LogError($"IFix 别慌，我是新增函数测试：IFixNewMethodTest，Interpret 里别搞匿名函数，不支持");
        Debug.LogError($"IFix 别慌，我是新增属性测试: iFixNewPropertyTest={iFixNewPropertyTest}");
        Debug.LogError($"IFix 别慌，我是新增字段测试: iFixNewFieldTest={iFixNewFieldTest}");
        new IFixNewClassTest();
        var tuple = new Tuple<bool, int, int, float, string>(false, 1,2,3f, "4s");
        Debug.LogError($"IFix 别慌，我是 Tuple 测试: b:{tuple.Item1} i:{tuple.Item2} i:{tuple.Item3} f:{tuple.Item4} s:{tuple.Item5}");
    }

    [IFix.Interpret]
    public class IFixNewClassTest
    {
        public IFixNewClassTest()
        {
            Debug.LogError("IFix 别慌，我是新增类测试:IFixNewClassTest");
        }
    }
#endif

    private void Update(){}
}