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
    public class QRCodeEncodeControllerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(QRCodeEncodeController);
			Utils.BeginObjectRegister(type, L, translator, 0, 3, 5, 5);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Encode", _m_Encode);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddLogoToQRCode", _m_AddLogoToQRCode);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "onQREncodeFinished", _e_onQREncodeFinished);
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "e_QRCodeWidth", _g_get_e_QRCodeWidth);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "e_QRCodeHeight", _g_get_e_QRCodeHeight);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "eCodeFormat", _g_get_eCodeFormat);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "e_QRLogoTex", _g_get_e_QRLogoTex);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "e_EmbedLogoRatio", _g_get_e_EmbedLogoRatio);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "e_QRCodeWidth", _s_set_e_QRCodeWidth);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "e_QRCodeHeight", _s_set_e_QRCodeHeight);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "eCodeFormat", _s_set_eCodeFormat);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "e_QRLogoTex", _s_set_e_QRLogoTex);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "e_EmbedLogoRatio", _s_set_e_EmbedLogoRatio);
            
			
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
					
					QRCodeEncodeController __cl_gen_ret = new QRCodeEncodeController();
					translator.Push(L, __cl_gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception __gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to QRCodeEncodeController constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Encode(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string valueStr = LuaAPI.lua_tostring(L, 2);
                    
                        int __cl_gen_ret = __cl_gen_to_be_invoked.Encode( valueStr );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddLogoToQRCode(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    __cl_gen_to_be_invoked.AddLogoToQRCode(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_e_QRCodeWidth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, __cl_gen_to_be_invoked.e_QRCodeWidth);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_e_QRCodeHeight(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, __cl_gen_to_be_invoked.e_QRCodeHeight);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_eCodeFormat(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                translator.Push(L, __cl_gen_to_be_invoked.eCodeFormat);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_e_QRLogoTex(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                translator.Push(L, __cl_gen_to_be_invoked.e_QRLogoTex);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_e_EmbedLogoRatio(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, __cl_gen_to_be_invoked.e_EmbedLogoRatio);
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_e_QRCodeWidth(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                __cl_gen_to_be_invoked.e_QRCodeWidth = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_e_QRCodeHeight(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                __cl_gen_to_be_invoked.e_QRCodeHeight = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_eCodeFormat(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                QRCodeEncodeController.CodeMode __cl_gen_value;translator.Get(L, 2, out __cl_gen_value);
				__cl_gen_to_be_invoked.eCodeFormat = __cl_gen_value;
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_e_QRLogoTex(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                __cl_gen_to_be_invoked.e_QRLogoTex = (UnityEngine.Texture2D)translator.GetObject(L, 2, typeof(UnityEngine.Texture2D));
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_e_EmbedLogoRatio(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                __cl_gen_to_be_invoked.e_EmbedLogoRatio = (float)LuaAPI.lua_tonumber(L, 2);
            
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            return 0;
        }
        
		
		
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _e_onQREncodeFinished(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    int __gen_param_count = LuaAPI.lua_gettop(L);
			QRCodeEncodeController __cl_gen_to_be_invoked = (QRCodeEncodeController)translator.FastGetCSObj(L, 1);
                QRCodeEncodeController.QREncodeFinished __gen_delegate = translator.GetDelegate<QRCodeEncodeController.QREncodeFinished>(L, 3);
                if (__gen_delegate == null) {
                    return LuaAPI.luaL_error(L, "#3 need QRCodeEncodeController.QREncodeFinished!");
                }
				
				if (__gen_param_count == 3)
				{
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "+")) {
						__cl_gen_to_be_invoked.onQREncodeFinished += __gen_delegate;
						return 0;
					} 
					
					
					if (LuaAPI.xlua_is_eq_str(L, 2, "-")) {
						__cl_gen_to_be_invoked.onQREncodeFinished -= __gen_delegate;
						return 0;
					} 
					
				}
			} catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
			LuaAPI.luaL_error(L, "invalid arguments to QRCodeEncodeController.onQREncodeFinished!");
            return 0;
        }
        
		
		
    }
}
