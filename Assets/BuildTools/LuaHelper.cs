using System;
using UnityEngine;
using XLua;

using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LuaHelper
{
    static LuaHelper mInstance = null;
    public LuaHelper Instance
    {
        get
        {
            return mInstance ?? (mInstance = new LuaHelper());
        }
    }

    static LuaEnv luaEnv = null;

#if UNITY_EDITOR
    public static bool mInited = false;
    [InitializeOnLoadMethod]
    public static bool Init()
    {
        luaEnv = new LuaEnv();
        luaEnv.AddLoader((ref string filename) =>
        {
            var assetName = "Assets/ABResources/Lua/" + filename.Replace(".", "/") + ProjectConfig.Instance().LuaExtension;
            //Debug.Log("<Color=green>lua: " + assetName + "</Color>");
            var bytes = File.ReadAllBytes(assetName);
            return bytes;
        });

        luaEnv.DoString("require 'utility.BridgingClass'");

        Debug.Log("<Color=green>EditorLuaHelper Init OK</Color>");
        mInited = true;
        return true;
    }
#endif

    public static TValue Get<TValue>(string tableName, int id, string key)
    {
        if(Init())
        {
            try
            {
                luaEnv.DoString("require 'Table." + tableName + "'");

                var grab = luaEnv.Global.GetInPath<LuaTableDelegate>("BridgingClass.GetTable");
                var val = (TValue)grab(tableName, id, key);
                return val;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
                return default(TValue);
            }
        }
        else
            return default(TValue);
    }
    public static string EditorText(int id)
    {
        return LuaHelper.Get<string>("EditorText", id, "zh");//FGEditorSetting.LanguageName
    }
}
