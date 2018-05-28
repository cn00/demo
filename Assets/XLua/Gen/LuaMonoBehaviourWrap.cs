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
    public class LuaMonoBehaviourWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(LuaMonoBehaviour);
			Utils.BeginObjectRegister(type, L, translator, 0, 3, 4, 2);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetLua", _m_SetLua);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "YieldAndCallback", _m_YieldAndCallback);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "WaitForSeconds", _m_WaitForSeconds);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "luaTable", _g_get_luaTable);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "Inited", _g_get_Inited);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "luaScript", _g_get_luaScript);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "injections", _g_get_injections);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "luaScript", _s_set_luaScript);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "injections", _s_set_injections);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					LuaMonoBehaviour __cl_gen_ret = new LuaMonoBehaviour();
					translator.Push(L, __cl_gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception __gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to LuaMonoBehaviour constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetLua(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string path = LuaAPI.lua_tostring(L, 2);
                    
                    __cl_gen_to_be_invoked.SetLua( path );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_YieldAndCallback(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    object to_yield = translator.GetObject(L, 2, typeof(object));
                    System.Action callback = translator.GetDelegate<System.Action>(L, 3);
                    
                    __cl_gen_to_be_invoked.YieldAndCallback( to_yield, callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_WaitForSeconds(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    float seconds = (float)LuaAPI.lua_tonumber(L, 2);
                    
                        System.Collections.IEnumerator __cl_gen_ret = __cl_gen_to_be_invoked.WaitForSeconds( seconds );
                        translator.PushAny(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_luaTable(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
                translator.Push(L, __cl_gen_to_be_invoked.luaTable);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Inited(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, __cl_gen_to_be_invoked.Inited);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_luaScript(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
                translator.Push(L, __cl_gen_to_be_invoked.luaScript);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_injections(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
                translator.Push(L, __cl_gen_to_be_invoked.injections);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_luaScript(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
                __cl_gen_to_be_invoked.luaScript = (LuaAsset)translator.GetObject(L, 2, typeof(LuaAsset));
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_injections(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                LuaMonoBehaviour __cl_gen_to_be_invoked = (LuaMonoBehaviour)translator.FastGetCSObj(L, 1);
                __cl_gen_to_be_invoked.injections = (LuaMonoBehaviour.Injection[])translator.GetObject(L, 2, typeof(LuaMonoBehaviour.Injection[]));
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
