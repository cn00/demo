using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using XLua;
using System.Runtime.InteropServices;
using System;

#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

// [ExecuteInEditMode]
public class LuaSys : SingleMono<LuaSys>
{
    [SerializeField]
    public string[] PackagePath = new []{
        "lua/",
        "lua/utility/",
        "lua/proto/",
    };
    const string Tag = "LuaSys";
    //all lua behaviour shared one luaenv only!
    internal LuaEnv luaEnv = new LuaEnv();
    public LuaEnv GlobalEnv
    {
        get
        {
            if(luaEnv == null)
            {
                luaEnv = new LuaEnv();
                // InitLuaEnv();
            }
            return luaEnv;
        }
    }

    public object[] DoString(string lua, string chunkName = "trunk")
    {
        return GlobalEnv.DoString(lua, chunkName);
    }

    public LuaTable Inject(LuaBehaviour lb)
    {
        LuaTable luaTable = luaEnv.NewTable();

        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        luaTable.SetMetaTable(meta);
        meta.Dispose();

        luaTable.Set("mono", lb);
        luaTable.Set("gameObject", lb.gameObject);
        luaTable.Set("transform", lb.transform);
        if(lb.Injections != null && lb.Injections.Count > 0)
        {
            foreach(var injection in lb.Injections.Where(i => i.obj != null))
            {
                luaTable.Set(injection.obj.name, injection.obj);
            }
        }
        if(lb.InjectValues != null && lb.InjectValues.Count > 0)
        {
            foreach(var injection in lb.InjectValues.Where(i => i.v != null))
            {
                luaTable.Set(injection.k, injection.v);
            }
        }
        return luaTable;
    }

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
        // AppLog.d(Tag, "require: " + assetName);
#if UNITY_EDITOR
        if(!BuildConfig.Instance().UseBundle)
        {
            if(File.Exists(BuildConfig.BundleResRoot + assetName))
            {
                bytes = File.ReadAllBytes(BuildConfig.BundleResRoot + assetName);
            }
        }
        else
#endif
        {
            var data = AssetSys.GetAssetSync<TextAsset>(assetName);
            if(data!=null)
                bytes = data.bytes;
        }

        if(bytes == null && retry < PackagePath.Length)
        {
            bytes = Require(luapath, PackagePath[retry], 1+retry);
        }
        return bytes;
    }

    public void InitLuaEnv()
    {
        luaEnv.AddLoader(Require);

        luaEnv.AddBuildin("socket.serial",  XLua.LuaDLL.Lua.LoadSocketSerial);
        luaEnv.AddBuildin("mime.core", XLua.LuaDLL.Lua.LoadSocketMime);
        luaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
        luaEnv.AddBuildin("ffi", XLua.LuaDLL.Lua.LoadFfi);

        luaEnv.AddBuildin("lfb", XLua.LuaDLL.Lua.LoadLfb);

        luaEnv.AddBuildin("nslua", XLua.LuaDLL.Lua.LoadNSLua);

        luaEnv.AddBuildin("p7zip", XLua.LuaDLL.Lua.LoadP7zip);

        luaEnv.AddBuildin("lsqlite3", XLua.LuaDLL.Lua.LoadLSQLite3);
        luaEnv.AddBuildin("luasql.mysql", XLua.LuaDLL.Lua.LoadLuasqlMysql);

        luaEnv.AddBuildin("lxp", XLua.LuaDLL.Lua.LoadLxp);

        #if UNITY_EDITOR
        luaEnv.Global.Set("UNITY_EDITOR", true);
        #endif
        #if UNITY_EDITOR_OSX
        luaEnv.Global.Set("UNITY_EDITOR_OSX", true);
        #endif
        #if UNITY_EDITOR_WIN
        luaEnv.Global.Set("UNITY_EDITOR_WIN", true);
        #endif

        #if UNITY_STANDALONE
        luaEnv.Global.Set("UNITY_STANDALONE", true);
        #endif
        #if UNITY_OSX
        luaEnv.Global.Set("UNITY_OSX", true);
        #endif
        #if UNITY_STANDALONE_WIN
        luaEnv.Global.Set("UNITY_STANDALONE_WIN", true);
        #endif

        #if UNITY_ANDROID
        luaEnv.Global.Set("UNITY_ANDROID", true);
        #endif
        #if UNITY_IOS
        luaEnv.Global.Set("UNITY_IOS", true);
        #endif
    }

    public override IEnumerator Init()
    {
        InitLuaEnv();
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
    public delegate object LuaFuncDelegate(object luaobj,object luaobj2);
    Dictionary<string, LuaFuncDelegate> mLuaFuncDelegate = new Dictionary<string, LuaFuncDelegate>();
    public LuaFuncDelegate GetLuaFunc(LuaTable self, string luaMethodPath)
    {
        LuaFuncDelegate luafun = null;
        if(!mLuaFuncDelegate.TryGetValue(luaMethodPath, out luafun))
        {
            luafun = mLuaFuncDelegate[luaMethodPath] = self.GetInPath<LuaFuncDelegate>(luaMethodPath);
        }
        return luafun;
    }
    public LuaFuncDelegate GetGLuaFunc(string luaMethodPath)
    {
        return GetLuaFunc(GlobalEnv.Global, luaMethodPath);
    }

    public LuaTable GetLuaTable(byte[] textBytes, LuaBehaviour self = null, string name = "LuaBehaviour")
    {
        LuaTable env = null;
        if(self != null)
            env = Inject(self);
        return GetLuaTable(textBytes, env, name);
    }

    public LuaTable GetLuaTable(byte[] textBytes, LuaTable env, string name)
    {
        //sweep utf bom
        AssetSys.TrimBom(ref textBytes);

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