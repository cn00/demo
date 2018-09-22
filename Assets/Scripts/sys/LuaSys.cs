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
    //all lua behaviour shared one luaenv only!
    internal LuaEnv luaEnv = new LuaEnv();
    public LuaEnv GlobalEnv
    {
        get { return luaEnv; }
    }

    public LuaTable NewEnv(LuaMonoBehaviour lb)
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
                // AppLog.d("injections:{0}:{1}", injection.obj.name, injection.obj);
            }
        }
        return luaTable;
    }

    public byte[] LuaLoader(string filename)
    {
        return LuaLoader(ref filename);
    }
    public byte[] LuaLoader(ref string filename)
    {
        if(string.IsNullOrEmpty(filename))
            return null;
            
        var LuaExtension = BuildConfig.LuaExtension;

        if(filename.EndsWith(LuaExtension))
        {
            filename = filename.Remove(filename.LastIndexOf(LuaExtension));
        }
        AppLog.d("require: " + filename);

        byte[] bytes = null;
#if UNITY_EDITOR
        if(BuildConfig.Instance().UseBundle)
#endif
        {
            var data = AssetSys.Instance.GetAssetSync<TextAsset>(filename.Replace(".", "/") + LuaExtension + ".txt");
            bytes = data.bytes;
        }
#if UNITY_EDITOR
        else
        {
            var assetName = BuildConfig.BundleResRoot + filename.Replace(".", "/") + LuaExtension;
            if(File.Exists(assetName))
            {
                bytes = File.ReadAllBytes(assetName);
            }
            else
            {
                AppLog.w(assetName + "not found.");
            }
        }
#endif
        return bytes;
    }

    private void Awake()
    {
        luaEnv.AddLoader(LuaLoader);
    }

    public override IEnumerator Init()
    {
        byte[] textBytes = null;
        yield return AssetSys.Instance.GetAsset<DataObject>("lua/utility/init.lua", asset => {
            textBytes = asset.Data;
        });
        GetLuaTable(textBytes);

        yield return base.Init();
    }

    public LuaTable GetLuaTable(byte[] textBytes, LuaMonoBehaviour self = null, string name = "LuaMonoBehaviour")
    {
        LuaTable env = null;
        if(self != null)
            env = NewEnv(self);
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
