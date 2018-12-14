namespace XLua.LuaDLL
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

    public partial class Lua
    {
        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_lpeg(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadLpeg(System.IntPtr L)
        {
            return luaopen_lpeg(L);
        }


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_ffi(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadFfi(System.IntPtr L)
        {
            return luaopen_ffi(L);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_mime_core(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadSocketMime(System.IntPtr L)
        {
            return luaopen_mime_core(L);
        }


        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_luasocket_scripts(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadSocketScripts(System.IntPtr L)
        {
            return luaopen_luasocket_scripts(L);
        }

        [DllImport(LUADLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_nslua(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadNSLua(System.IntPtr L)
        {
            return luaopen_nslua(L);
        }

    }
}