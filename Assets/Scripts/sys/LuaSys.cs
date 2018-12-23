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

public class LuaSys : SingleMono<LuaSys>
{
    [SerializeField]
    public string[] PackagePath = new []{
        "lua/proto/",
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
        if(BuildConfig.Instance().UseBundle)
#endif
        {
            var data = AssetSys.Instance.GetAssetSync<TextAsset>(assetName + ".txt");
            if(data!=null)
                bytes = data.bytes;
        }
#if UNITY_EDITOR
        else
        {
            if(File.Exists(BuildConfig.BundleResRoot + assetName))
            {
                bytes = File.ReadAllBytes(BuildConfig.BundleResRoot + assetName);
            }
        }
#endif
        if(bytes == null && retry < PackagePath.Length)
        {
            bytes = Require(luapath, PackagePath[retry], 1+retry);
        }
        return bytes;
    }

    public override IEnumerator Init()
    {
        luaEnv.AddLoader(Require);

        // luaEnv.AddBuildin("socket.socket", XLua.StaticLuaCallbacks.LoadSocketCore);
        // luaEnv.AddBuildin("socket.util", XLua.LuaDLL.Lua.LoadSocketScripts);
        luaEnv.AddBuildin("mime.core", XLua.LuaDLL.Lua.LoadSocketMime);
        luaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
        luaEnv.AddBuildin("ffi", XLua.LuaDLL.Lua.LoadFfi);

        luaEnv.AddBuildin("lfb", XLua.LuaDLL.Lua.LoadLfb);

        luaEnv.AddBuildin("nslua", XLua.LuaDLL.Lua.LoadNSLua);

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
