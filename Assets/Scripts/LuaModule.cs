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
        [DllImport("lpeg", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_lpeg(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadLpeg(System.IntPtr L)
        {
            return luaopen_lpeg(L);
        }


        [DllImport("ffi", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_ffi(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadFfi(System.IntPtr L)
        {
            return luaopen_ffi(L);
        }

        [DllImport("lfb", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_lfb(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadLfb(System.IntPtr L)
        {
            return luaopen_lfb(L);
        }

        [DllImport("mime", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_mime_core(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadSocketMime(System.IntPtr L)
        {
            return luaopen_mime_core(L);
        }


        [DllImport("serial", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_socket_serial(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadSocketSerial(System.IntPtr L)
        {
            return luaopen_socket_serial(L);
        }

        [DllImport("nslua", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_nslua(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadNSLua(System.IntPtr L)
        {
            return luaopen_nslua(L);
        }


        //luaopen_p7zip
        #if UNITY_EDITOR_OSX || UNITY_OSX
        public const string P7ZIP_DLL = "p7zip"; // ok
        public const string LUASQLITE_DLL = "lsqlite3"; // ok
        public const string LXP_DLL = "lxp"; // ok
        // #elif UNITY_IOS
        // public const string P7ZIP_DLL = "@rpath/p7zip.framework/p7zip";
        #else // if UNITY_ANDROID
        public const string P7ZIP_DLL = "p7zip";
        public const string LUASQLITE_DLL = "lsqlite3";
        public const string LXP_DLL = "lxp"; // ok
        #endif
        [DllImport(P7ZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_p7zip(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadP7zip(System.IntPtr L)
        {
            return luaopen_p7zip(L);
        }
        [DllImport(P7ZIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int p7zip_executeCommand(string cmd);


        [DllImport("luasql", CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_luasql_mysql(System.IntPtr L);
        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadLuasqlMysql(System.IntPtr L)
        {
            return luaopen_luasql_mysql(L);
        }

        [DllImport(LUASQLITE_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_lsqlite3(System.IntPtr L);
        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadLSQLite3(System.IntPtr L)
        {
            return luaopen_lsqlite3(L);
        }


        [DllImport(LXP_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int luaopen_lxp(System.IntPtr L);

        [MonoPInvokeCallback(typeof(LuaCSFunction))]
        public static int LoadLxp(System.IntPtr L)
        {
            return luaopen_lxp(L);
        }
    }
}