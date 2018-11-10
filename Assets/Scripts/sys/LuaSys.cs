using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using XLua;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LuaSys : SingleMono<LuaSys>
{
    [SerializeField]
    public string[] PackagePath = new []{
        "lua/plugins/",
        "lua/utility/"
    };
    const string Tag = "LuaSys";
    //all lua behaviour shared one luaenv only!
    internal LuaEnv luaEnv = new LuaEnv();
    public LuaEnv GlobalEnv
    {
        get { return luaEnv; }
    }

    public LuaTable Inject(LuaMonoBehaviour lb)
    {
        LuaTable luaTable = luaEnv.NewTable();

        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        luaTable.SetMetaTable(meta);
        meta.Dispose();

        luaTable.Set("mono", lb);
        if(lb.injections != null && lb.injections.Length > 0)
        {
            foreach(var injection in lb.injections.Where(i => i.obj != null))
            {
                luaTable.Set(injection.obj.name, injection.obj);
                // AppLog.d(Tag, "injections:{0}:{1}", injection.obj.name, injection.obj);
            }
        }
        return luaTable;
    }

    // public byte[] Require(string filename)
    // {
    //     return Require(ref filename);
    // }
    public byte[] Require(ref string luapath)
    {
        return Require(luapath);
    }
    public byte[] Require(string luapath, string search = "", int retry = 0)
    {
        if(string.IsNullOrEmpty(luapath))
            return null;
            
        var LuaExtension = BuildConfig.LuaExtension;

        if(luapath.EndsWith(LuaExtension))
        {
            luapath = luapath.Remove(luapath.LastIndexOf(LuaExtension));
        }

        byte[] bytes = null;
        var assetName = search + luapath.Replace(".", "/") + LuaExtension;
        // var assetName2 = "lua/plugins/" + luapath.Replace(".", "/") + LuaExtension;
        AppLog.d(Tag, "require: " + assetName);
#if UNITY_EDITOR
        if(BuildConfig.Instance().UseBundle)
#endif
        {
            var data = AssetSys.Instance.GetAssetSync<TextAsset>(assetName + ".txt");
            // if(data == null)
            // {
            //     data = AssetSys.Instance.GetAssetSync<TextAsset>(assetName2 + ".txt");
            // }
            if(data!=null)
                bytes = data.bytes;
            // else
            //     AppLog.e(Tag, luapath + " lua not found");
        }
#if UNITY_EDITOR
        else
        {
            if(File.Exists(BuildConfig.BundleResRoot + assetName))
            {
                bytes = File.ReadAllBytes(BuildConfig.BundleResRoot + assetName);
            }
            // else if(File.Exists(BuildConfig.BundleResRoot + assetName2))
            // {
            //     bytes = File.ReadAllBytes(BuildConfig.BundleResRoot + assetName2);
            // }
            // else
            // {
            //     AppLog.e(Tag, "[{0}] or [{1}] not found.", assetName, assetName2);
            // }
        }
#endif
        if(bytes == null && retry < PackagePath.Length)
        {
            bytes = Require(luapath, PackagePath[retry], ++retry);
            AppLog.d(Tag, "filaly: retry={0}, luapath={1}", retry, PackagePath[retry] + luapath);
            // if(retry == 0)
            //     bytes = Require("lua/plugins/" + luapath, ++retry);
            // if(retry == 1)
            //     bytes = Require("lua/utility/" + luapath, ++retry);
        }
        return bytes;
    }

    public override IEnumerator Init()
    {
        luaEnv.AddLoader(Require);
        yield return base.Init();
    }

    // public override IEnumerator Init()
    // {
    //     byte[] textBytes = null;
    //     yield return AssetSys.Instance.GetAsset<DataObject>("lua/utility/init.lua", asset => {
    //         textBytes = asset.Data;
    //     });
    //     GetLuaTable(textBytes);

    //     yield return base.Init();
    // }

    [CSharpCallLua]
    public delegate object[] TableDelegate(LuaTable table);
    public LuaTable GetMetaTable(LuaTable table)
    {
        var tabledelegate = GlobalEnv.Global.GetInPath<TableDelegate>("BridgingClass.GetMetaTable");
        LuaTable fileData = null;
        object[] ret = tabledelegate(table);
        if(ret != null && ret.Length > 0)
            fileData = ret[0] as LuaTable;
        return fileData;
    }
    public LuaTable GetLuaTable(byte[] textBytes, LuaMonoBehaviour self = null, string name = "LuaMonoBehaviour")
    {
        LuaTable env = null;
        if(self != null)
            env = Inject(self);
        return GetLuaTable(textBytes, env, name);
    }

    public LuaTable GetLuaTable(byte[] textBytes, LuaTable env, string name)
    {
        //sweep utf bom
        if(textBytes[0] == 0xef)
        {
            textBytes[0] = (byte)' ';
            textBytes[1] = (byte)' ';
            textBytes[2] = (byte)' ';
        }

        var table = luaEnv.DoString(Encoding.UTF8.GetString(textBytes), name, env);
        if(table != null && table.Length > 0)
        {
            var luaTable = table[0] as LuaTable;
            return luaTable;
        }
        else
        {
            return null;
        }
    }
}
