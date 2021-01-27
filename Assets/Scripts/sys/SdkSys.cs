using System;
using UnityEngine;
using XLua;

namespace App
{
    public class SdkSys : SingleMono<SdkSys>
    {
        private LuaSys.LuaFuncDelegate luaJsonDecoder = null;
        public void NativeMessageHandler(string jsonstr)
        {
            if(luaJsonDecoder == null)
                luaJsonDecoder = LuaSys.Instance.GetGLuaFunc("json.decode");
            Debug.Assert(luaJsonDecoder!= null, $"luaJsonDecoder not found");
            
            LuaTable lt = luaJsonDecoder(jsonstr, null) as LuaTable;
            Debug.Assert(lt != null, $"json to lua decode err:[{jsonstr}]");
            
            var type = lt.Get<string>("type") ?? "";
            var gluafunc = "OnNativeMessage" + type;
            var luaf = LuaSys.Instance.GetGLuaFunc(gluafunc);
            if (luaf != null)
                luaf(null, lt);
            else
                Debug.LogError($"global lua function [{gluafunc}] not found");
        }
    }
}