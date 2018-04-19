#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class SQLiteSQLite3Wrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(SQLite.SQLite3);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 42, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Threadsafe", _m_Threadsafe_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Open", _m_Open_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Open16", _m_Open16_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "EnableLoadExtension", _m_EnableLoadExtension_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Close", _m_Close_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Close2", _m_Close2_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Initialize", _m_Initialize_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Shutdown", _m_Shutdown_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Config", _m_Config_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetDirectory", _m_SetDirectory_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BusyTimeout", _m_BusyTimeout_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Changes", _m_Changes_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Prepare2", _m_Prepare2_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Step", _m_Step_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Reset", _m_Reset_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Finalize", _m_Finalize_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LastInsertRowid", _m_LastInsertRowid_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Errmsg", _m_Errmsg_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetErrmsg", _m_GetErrmsg_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BindParameterIndex", _m_BindParameterIndex_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BindNull", _m_BindNull_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BindInt", _m_BindInt_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BindInt64", _m_BindInt64_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BindDouble", _m_BindDouble_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BindText", _m_BindText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "BindBlob", _m_BindBlob_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnCount", _m_ColumnCount_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnName", _m_ColumnName_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnName16", _m_ColumnName16_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnType", _m_ColumnType_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnInt", _m_ColumnInt_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnInt64", _m_ColumnInt64_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnDouble", _m_ColumnDouble_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnText", _m_ColumnText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnText16", _m_ColumnText16_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnBlob", _m_ColumnBlob_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnBytes", _m_ColumnBytes_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnString", _m_ColumnString_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ColumnByteArray", _m_ColumnByteArray_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ExtendedErrCode", _m_ExtendedErrCode_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LibVersionNumber", _m_LibVersionNumber_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "SQLite.SQLite3 does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Threadsafe_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        int __cl_gen_ret = SQLite.SQLite3.Threadsafe(  );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Open_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string filename = LuaAPI.lua_tostring(L, 1);
                    System.IntPtr db;
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Open( filename, out db );
                        translator.Push(L, __cl_gen_ret);
                    LuaAPI.lua_pushlightuserdata(L, db);
                        
                    
                    
                    
                    return 2;
                }
                if(__gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<SQLite.SQLiteOpenFlags>(L, 2)&& LuaTypes.LUA_TLIGHTUSERDATA == LuaAPI.lua_type(L, 3)) 
                {
                    string filename = LuaAPI.lua_tostring(L, 1);
                    System.IntPtr db;
                    SQLite.SQLiteOpenFlags flags;translator.Get(L, 2, out flags);
                    System.IntPtr zvfs = LuaAPI.lua_touserdata(L, 3);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Open( filename, out db, flags, zvfs );
                        translator.Push(L, __cl_gen_ret);
                    LuaAPI.lua_pushlightuserdata(L, db);
                        
                    
                    
                    
                    return 2;
                }
                if(__gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<SQLite.SQLiteOpenFlags>(L, 2)&& LuaTypes.LUA_TLIGHTUSERDATA == LuaAPI.lua_type(L, 3)) 
                {
                    byte[] filename = LuaAPI.lua_tobytes(L, 1);
                    System.IntPtr db;
                    SQLite.SQLiteOpenFlags flags;translator.Get(L, 2, out flags);
                    System.IntPtr zvfs = LuaAPI.lua_touserdata(L, 3);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Open( filename, out db, flags, zvfs );
                        translator.Push(L, __cl_gen_ret);
                    LuaAPI.lua_pushlightuserdata(L, db);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to SQLite.SQLite3.Open!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Open16_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string filename = LuaAPI.lua_tostring(L, 1);
                    System.IntPtr db;
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Open16( filename, out db );
                        translator.Push(L, __cl_gen_ret);
                    LuaAPI.lua_pushlightuserdata(L, db);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_EnableLoadExtension_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    int onoff = LuaAPI.xlua_tointeger(L, 2);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.EnableLoadExtension( db, onoff );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Close_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Close( db );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Close2_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Close2( db );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Initialize_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Initialize(  );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Shutdown_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Shutdown(  );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Config_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    SQLite.SQLite3.ConfigOption option;translator.Get(L, 1, out option);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Config( option );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetDirectory_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    uint directoryType = LuaAPI.xlua_touint(L, 1);
                    string directoryPath = LuaAPI.lua_tostring(L, 2);
                    
                        int __cl_gen_ret = SQLite.SQLite3.SetDirectory( directoryType, directoryPath );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BusyTimeout_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    int milliseconds = LuaAPI.xlua_tointeger(L, 2);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.BusyTimeout( db, milliseconds );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Changes_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    
                        int __cl_gen_ret = SQLite.SQLite3.Changes( db );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Prepare2_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 2&& LuaTypes.LUA_TLIGHTUSERDATA == LuaAPI.lua_type(L, 1)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)) 
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    string query = LuaAPI.lua_tostring(L, 2);
                    
                        System.IntPtr __cl_gen_ret = SQLite.SQLite3.Prepare2( db, query );
                        LuaAPI.lua_pushlightuserdata(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 4&& LuaTypes.LUA_TLIGHTUSERDATA == LuaAPI.lua_type(L, 1)&& (LuaAPI.lua_isnil(L, 2) || LuaAPI.lua_type(L, 2) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TLIGHTUSERDATA == LuaAPI.lua_type(L, 4)) 
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    string sql = LuaAPI.lua_tostring(L, 2);
                    int numBytes = LuaAPI.xlua_tointeger(L, 3);
                    System.IntPtr stmt;
                    System.IntPtr pzTail = LuaAPI.lua_touserdata(L, 4);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Prepare2( db, sql, numBytes, out stmt, pzTail );
                        translator.Push(L, __cl_gen_ret);
                    LuaAPI.lua_pushlightuserdata(L, stmt);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to SQLite.SQLite3.Prepare2!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Step_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Step( stmt );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Reset_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Reset( stmt );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Finalize_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    
                        SQLite.SQLite3.Result __cl_gen_ret = SQLite.SQLite3.Finalize( stmt );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LastInsertRowid_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    
                        long __cl_gen_ret = SQLite.SQLite3.LastInsertRowid( db );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Errmsg_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    
                        System.IntPtr __cl_gen_ret = SQLite.SQLite3.Errmsg( db );
                        LuaAPI.lua_pushlightuserdata(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetErrmsg_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    
                        string __cl_gen_ret = SQLite.SQLite3.GetErrmsg( db );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BindParameterIndex_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    string name = LuaAPI.lua_tostring(L, 2);
                    
                        int __cl_gen_ret = SQLite.SQLite3.BindParameterIndex( stmt, name );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BindNull_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        int __cl_gen_ret = SQLite.SQLite3.BindNull( stmt, index );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BindInt_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    int val = LuaAPI.xlua_tointeger(L, 3);
                    
                        int __cl_gen_ret = SQLite.SQLite3.BindInt( stmt, index, val );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BindInt64_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    long val = LuaAPI.lua_toint64(L, 3);
                    
                        int __cl_gen_ret = SQLite.SQLite3.BindInt64( stmt, index, val );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BindDouble_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    double val = LuaAPI.lua_tonumber(L, 3);
                    
                        int __cl_gen_ret = SQLite.SQLite3.BindDouble( stmt, index, val );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BindText_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    string val = LuaAPI.lua_tostring(L, 3);
                    int n = LuaAPI.xlua_tointeger(L, 4);
                    System.IntPtr free = LuaAPI.lua_touserdata(L, 5);
                    
                        int __cl_gen_ret = SQLite.SQLite3.BindText( stmt, index, val, n, free );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_BindBlob_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    byte[] val = LuaAPI.lua_tobytes(L, 3);
                    int n = LuaAPI.xlua_tointeger(L, 4);
                    System.IntPtr free = LuaAPI.lua_touserdata(L, 5);
                    
                        int __cl_gen_ret = SQLite.SQLite3.BindBlob( stmt, index, val, n, free );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnCount_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    
                        int __cl_gen_ret = SQLite.SQLite3.ColumnCount( stmt );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnName_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        System.IntPtr __cl_gen_ret = SQLite.SQLite3.ColumnName( stmt, index );
                        LuaAPI.lua_pushlightuserdata(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnName16_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        string __cl_gen_ret = SQLite.SQLite3.ColumnName16( stmt, index );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnType_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        SQLite.SQLite3.ColType __cl_gen_ret = SQLite.SQLite3.ColumnType( stmt, index );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnInt_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        int __cl_gen_ret = SQLite.SQLite3.ColumnInt( stmt, index );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnInt64_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        long __cl_gen_ret = SQLite.SQLite3.ColumnInt64( stmt, index );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnDouble_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        double __cl_gen_ret = SQLite.SQLite3.ColumnDouble( stmt, index );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnText_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        System.IntPtr __cl_gen_ret = SQLite.SQLite3.ColumnText( stmt, index );
                        LuaAPI.lua_pushlightuserdata(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnText16_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        System.IntPtr __cl_gen_ret = SQLite.SQLite3.ColumnText16( stmt, index );
                        LuaAPI.lua_pushlightuserdata(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnBlob_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        System.IntPtr __cl_gen_ret = SQLite.SQLite3.ColumnBlob( stmt, index );
                        LuaAPI.lua_pushlightuserdata(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnBytes_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        int __cl_gen_ret = SQLite.SQLite3.ColumnBytes( stmt, index );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnString_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        string __cl_gen_ret = SQLite.SQLite3.ColumnString( stmt, index );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ColumnByteArray_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    System.IntPtr stmt = LuaAPI.lua_touserdata(L, 1);
                    int index = LuaAPI.xlua_tointeger(L, 2);
                    
                        byte[] __cl_gen_ret = SQLite.SQLite3.ColumnByteArray( stmt, index );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ExtendedErrCode_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    System.IntPtr db = LuaAPI.lua_touserdata(L, 1);
                    
                        SQLite.SQLite3.ExtendedResult __cl_gen_ret = SQLite.SQLite3.ExtendedErrCode( db );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LibVersionNumber_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        int __cl_gen_ret = SQLite.SQLite3.LibVersionNumber(  );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
