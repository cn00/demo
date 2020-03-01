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
        public static extern int luaopen_lfb(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadLfb(System.IntPtr L)
        {
            return luaopen_lfb(L);
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

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_nslua(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadNSLua(System.IntPtr L)
        {
            return luaopen_nslua(L);
        }


        //luaopen_p7zip
        #if UNITY_EDITOR_OSX || UNITY_OSX
        public const string P7ZIP_DLL = "Assets/XLua/Plugins/OSX/libp7zip.so"; // ok
        #else // ! OSX
        public const string P7ZIP_DLL = "p7zip";
        #endif
        [DllImport(P7ZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_p7zip(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadP7zip(System.IntPtr L)
        {
            return luaopen_p7zip(L);
        }
        
        // luaopen_bit32
        #if UNITY_EDITOR_OSX || UNITY_OSX
        public const string LUASQLITE_DLL = "Assets/XLua/Plugins/OSX/liblsqlite3.so"; // ok
        #else // ! OSX
        public const string LUASQLITE_DLL = "lsqlite3";
        #endif
        [DllImport(LUASQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_lsqlite3(System.IntPtr L);
        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadLSQLite3(System.IntPtr L)
        {
            return luaopen_lsqlite3(L);
        }

    }
}