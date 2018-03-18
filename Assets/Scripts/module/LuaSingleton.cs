using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LuaSingleton : SingletonMB<LuaSingleton>
{
    //all lua behaviour shared one luaenv only!
    internal static LuaEnv luaEnv = new LuaEnv();
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

        luaTable.Set("self", lb);
        foreach(var injection in lb.injections)
        {
            luaTable.Set(injection.name, injection);
        }

        return luaTable;
    }
    public byte[] LuaLoaderEditor(ref string filename)
    {
        var LuaExtension = ProjectConfig.Instance.LuaExtension;
        if(filename.EndsWith(LuaExtension))
        {
            filename = filename.Remove(filename.LastIndexOf(LuaExtension));
        }
        if(ProjectConfig.Instance.UseBundle)
        {
            var assetName = BundleConfig.ABResourceRoot + filename.Replace(".", "/") + LuaExtension;
            //Debug.Log("<Color=green>lua: " + assetName + "</Color>");
            var bytes = File.ReadAllBytes(assetName);
            return bytes;
        }
        else
        {
            return null;
        }
    }

    public byte[] LuaLoader(ref string filename)
    {
        var assetName = BundleConfig.ABResourceRoot + filename.Replace(".", "/") + ProjectConfig.Instance.LuaExtension;
        var bundleRoot = filename.Substring(0, filename.IndexOf('/'));
        var bundlePath = "";
        var bundle = AssetBundle.LoadFromFile(bundlePath);
        var textAsset = bundle.LoadAsset<TextAsset>(filename);
        var bytes = textAsset.bytes;
        return bytes;
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    public override IEnumerator Init()
    {
#if UNITY_EDITOR
        luaEnv.AddLoader(LuaLoaderEditor);
#else
        luaEnv.AddLoader(LuaLoader);
#endif
        luaEnv.DoString("require 'lua.utility.BridgingClass'");

        return base.Init();
    }

    byte[] GetLuaBytes(string filename)
    {
        byte[] textBytes = null;
#if UNITY_EDITOR
        textBytes = LuaLoaderEditor(ref filename);
#else
        textBytes = LuaLoader(ref filename);
#endif
        return textBytes;
    }

    public LuaTable GetLuaTable(string filename, LuaMonoBehaviour self, string name = "LuaMonoBehaviour")
    {
        var env = NewEnv(self);
        return GetLuaTable(filename, env, name);
    }

    public LuaTable GetLuaTable(string filename, LuaTable env, string name)
    {
        byte[] textBytes = null;

        LuaMonoBehaviour self = env.Get<LuaMonoBehaviour>("self");
        if(self != null && self.Bundle != null)
            textBytes = self.Bundle.LoadABAsset<TextAsset>(filename + ".txt").bytes;
        else
            textBytes = GetLuaBytes(filename);
        //sweep utf bom
        if(textBytes[0] == 0xef)
        {
            textBytes[0] = (byte)' ';
            textBytes[1] = (byte)' ';
            textBytes[2] = (byte)' ';
        }

        var lua = luaEnv.DoString(Encoding.UTF8.GetString(textBytes), name, env);
        //env.Dispose();
        var luaTable = lua[0] as LuaTable;
        return luaTable;
    }
}
