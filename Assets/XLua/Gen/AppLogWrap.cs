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
    public class AppLogWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(AppLog);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 4, 4, 3);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "d", _m_d_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "w", _m_w_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "e", _m_e_xlua_st_);
            
			
            
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "TAG", _g_get_TAG);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Port", _g_get_Port);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "isEditor", _g_get_isEditor);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "TAG", _s_set_TAG);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "Port", _s_set_Port);
            Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "isEditor", _s_set_isEditor);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "AppLog does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_d_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string log = LuaAPI.lua_tostring(L, 1);
                    
                    AppLog.d( log );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count >= 0&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 1) || translator.Assignable<object>(L, 1))) 
                {
                    object[] args = translator.GetParams<object>(L, 1);
                    
                    AppLog.d( args );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count >= 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 2) || translator.Assignable<object>(L, 2))) 
                {
                    string fmt = LuaAPI.lua_tostring(L, 1);
                    object[] args = translator.GetParams<object>(L, 2);
                    
                    AppLog.d( fmt, args );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to AppLog.d!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_w_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string log = LuaAPI.lua_tostring(L, 1);
                    
                    AppLog.w( log );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count >= 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 2) || translator.Assignable<object>(L, 2))) 
                {
                    string fmt = LuaAPI.lua_tostring(L, 1);
                    object[] args = translator.GetParams<object>(L, 2);
                    
                    AppLog.w( fmt, args );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to AppLog.w!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_e_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count >= 0&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 1) || translator.Assignable<object>(L, 1))) 
                {
                    object[] args = translator.GetParams<object>(L, 1);
                    
                    AppLog.e( args );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.Exception>(L, 1)) 
                {
                    System.Exception ex = (System.Exception)translator.GetObject(L, 1, typeof(System.Exception));
                    
                    AppLog.e( ex );
                    
                    
                    
                    return 0;
                }
                if(__gen_param_count >= 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& (LuaTypes.LUA_TNONE == LuaAPI.lua_type(L, 2) || translator.Assignable<object>(L, 2))) 
                {
                    string fmt = LuaAPI.lua_tostring(L, 1);
                    object[] args = translator.GetParams<object>(L, 2);
                    
                    AppLog.e( fmt, args );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to AppLog.e!");
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TAG(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, AppLog.TAG);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Port(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.xlua_pushinteger(L, AppLog.Port);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isEditor(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushboolean(L, AppLog.isEditor);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_TAG(RealStatePtr L)
        {
		    try {
                
			    AppLog.TAG = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_Port(RealStatePtr L)
        {
		    try {
                
			    AppLog.Port = LuaAPI.xlua_tointeger(L, 1);
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_isEditor(RealStatePtr L)
        {
		    try {
                
			    AppLog.isEditor = LuaAPI.lua_toboolean(L, 1);
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
