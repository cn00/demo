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
    public class SystemConvertWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(System.Convert);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 24, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "GetTypeCode", _m_GetTypeCode_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsDBNull", _m_IsDBNull_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ChangeType", _m_ChangeType_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToBoolean", _m_ToBoolean_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToChar", _m_ToChar_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToSByte", _m_ToSByte_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToByte", _m_ToByte_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToInt16", _m_ToInt16_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToUInt16", _m_ToUInt16_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToInt32", _m_ToInt32_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToUInt32", _m_ToUInt32_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToInt64", _m_ToInt64_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToUInt64", _m_ToUInt64_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToSingle", _m_ToSingle_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToDouble", _m_ToDouble_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToDecimal", _m_ToDecimal_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToDateTime", _m_ToDateTime_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToString", _m_ToString_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToBase64String", _m_ToBase64String_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ToBase64CharArray", _m_ToBase64CharArray_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FromBase64String", _m_FromBase64String_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FromBase64CharArray", _m_FromBase64CharArray_xlua_st_);
            
			
            Utils.RegisterObject(L, translator, Utils.CLS_IDX, "DBNull", System.Convert.DBNull);
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "System.Convert does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetTypeCode_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        System.TypeCode __cl_gen_ret = System.Convert.GetTypeCode( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsDBNull_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        bool __cl_gen_ret = System.Convert.IsDBNull( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ChangeType_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.TypeCode>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.TypeCode typeCode;translator.Get(L, 2, out typeCode);
                    
                        object __cl_gen_ret = System.Convert.ChangeType( value, typeCode );
                        translator.PushAny(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.Type>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.Type conversionType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    
                        object __cl_gen_ret = System.Convert.ChangeType( value, conversionType );
                        translator.PushAny(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 3&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.TypeCode>(L, 2)&& translator.Assignable<System.IFormatProvider>(L, 3)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.TypeCode typeCode;translator.Get(L, 2, out typeCode);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 3, typeof(System.IFormatProvider));
                    
                        object __cl_gen_ret = System.Convert.ChangeType( value, typeCode, provider );
                        translator.PushAny(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 3&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.Type>(L, 2)&& translator.Assignable<System.IFormatProvider>(L, 3)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.Type conversionType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 3, typeof(System.IFormatProvider));
                    
                        object __cl_gen_ret = System.Convert.ChangeType( value, conversionType, provider );
                        translator.PushAny(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ChangeType!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToBoolean_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value, provider );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        bool __cl_gen_ret = System.Convert.ToBoolean( value, provider );
                        LuaAPI.lua_pushboolean(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToBoolean!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToChar_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        char __cl_gen_ret = System.Convert.ToChar( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        char __cl_gen_ret = System.Convert.ToChar( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        char __cl_gen_ret = System.Convert.ToChar( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToChar!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToSByte_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    int fromBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value, fromBase );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        sbyte __cl_gen_ret = System.Convert.ToSByte( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToSByte!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToByte_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    int fromBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value, fromBase );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        byte __cl_gen_ret = System.Convert.ToByte( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToByte!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToInt16_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    int fromBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value, fromBase );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        short __cl_gen_ret = System.Convert.ToInt16( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToInt16!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToUInt16_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    int fromBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value, fromBase );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        ushort __cl_gen_ret = System.Convert.ToUInt16( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToUInt16!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToInt32_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    int fromBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value, fromBase );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        int __cl_gen_ret = System.Convert.ToInt32( value, provider );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToInt32!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToUInt32_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    int fromBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value, fromBase );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value, provider );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        uint __cl_gen_ret = System.Convert.ToUInt32( value, provider );
                        LuaAPI.xlua_pushuint(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToUInt32!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToInt64_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    int fromBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value, fromBase );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value, provider );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        long __cl_gen_ret = System.Convert.ToInt64( value, provider );
                        LuaAPI.lua_pushint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToInt64!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToUInt64_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    int fromBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value, fromBase );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value, provider );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        ulong __cl_gen_ret = System.Convert.ToUInt64( value, provider );
                        LuaAPI.lua_pushuint64(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToUInt64!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToSingle_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value, provider );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        float __cl_gen_ret = System.Convert.ToSingle( value, provider );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToSingle!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToDouble_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value, provider );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        double __cl_gen_ret = System.Convert.ToDouble( value, provider );
                        LuaAPI.lua_pushnumber(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToDouble!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToDecimal_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value, provider );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        decimal __cl_gen_ret = System.Convert.ToDecimal( value, provider );
                        translator.PushDecimal(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToDecimal!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToDateTime_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value, provider );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        System.DateTime __cl_gen_ret = System.Convert.ToDateTime( value, provider );
                        translator.Push(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToDateTime!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToString_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    int toBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        string __cl_gen_ret = System.Convert.ToString( value, toBase );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    int toBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        string __cl_gen_ret = System.Convert.ToString( value, toBase );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    int toBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        string __cl_gen_ret = System.Convert.ToString( value, toBase );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    int toBase = LuaAPI.xlua_tointeger(L, 2);
                    
                        string __cl_gen_ret = System.Convert.ToString( value, toBase );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<object>(L, 1)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& translator.Assignable<System.DateTime>(L, 1)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToString( value );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    bool value = LuaAPI.lua_toboolean(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    char value = (char)LuaAPI.xlua_tointeger(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    sbyte value = (sbyte)LuaAPI.xlua_tointeger(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    byte value = (byte)LuaAPI.xlua_tointeger(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    short value = (short)LuaAPI.xlua_tointeger(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    ushort value = (ushort)LuaAPI.xlua_tointeger(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    int value = LuaAPI.xlua_tointeger(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    uint value = LuaAPI.xlua_touint(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isint64(L, 1))&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    long value = LuaAPI.lua_toint64(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || LuaAPI.lua_isuint64(L, 1))&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    ulong value = LuaAPI.lua_touint64(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    float value = (float)LuaAPI.lua_tonumber(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    double value = LuaAPI.lua_tonumber(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<object>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    object value = translator.GetObject(L, 1, typeof(object));
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 1) || translator.IsDecimal(L, 1))&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    decimal value;translator.Get(L, 1, out value);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& translator.Assignable<System.DateTime>(L, 1)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    System.DateTime value;translator.Get(L, 1, out value);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.IFormatProvider>(L, 2)) 
                {
                    string value = LuaAPI.lua_tostring(L, 1);
                    System.IFormatProvider provider = (System.IFormatProvider)translator.GetObject(L, 2, typeof(System.IFormatProvider));
                    
                        string __cl_gen_ret = System.Convert.ToString( value, provider );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToString!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToBase64String_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 1&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)) 
                {
                    byte[] inArray = LuaAPI.lua_tobytes(L, 1);
                    
                        string __cl_gen_ret = System.Convert.ToBase64String( inArray );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 3&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    byte[] inArray = LuaAPI.lua_tobytes(L, 1);
                    int offset = LuaAPI.xlua_tointeger(L, 2);
                    int length = LuaAPI.xlua_tointeger(L, 3);
                    
                        string __cl_gen_ret = System.Convert.ToBase64String( inArray, offset, length );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 2&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& translator.Assignable<System.Base64FormattingOptions>(L, 2)) 
                {
                    byte[] inArray = LuaAPI.lua_tobytes(L, 1);
                    System.Base64FormattingOptions options;translator.Get(L, 2, out options);
                    
                        string __cl_gen_ret = System.Convert.ToBase64String( inArray, options );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 4&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<System.Base64FormattingOptions>(L, 4)) 
                {
                    byte[] inArray = LuaAPI.lua_tobytes(L, 1);
                    int offset = LuaAPI.xlua_tointeger(L, 2);
                    int length = LuaAPI.xlua_tointeger(L, 3);
                    System.Base64FormattingOptions options;translator.Get(L, 4, out options);
                    
                        string __cl_gen_ret = System.Convert.ToBase64String( inArray, offset, length, options );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToBase64String!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ToBase64CharArray_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int __gen_param_count = LuaAPI.lua_gettop(L);
            
                if(__gen_param_count == 5&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<char[]>(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    byte[] inArray = LuaAPI.lua_tobytes(L, 1);
                    int offsetIn = LuaAPI.xlua_tointeger(L, 2);
                    int length = LuaAPI.xlua_tointeger(L, 3);
                    char[] outArray = (char[])translator.GetObject(L, 4, typeof(char[]));
                    int offsetOut = LuaAPI.xlua_tointeger(L, 5);
                    
                        int __cl_gen_ret = System.Convert.ToBase64CharArray( inArray, offsetIn, length, outArray, offsetOut );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                if(__gen_param_count == 6&& (LuaAPI.lua_isnil(L, 1) || LuaAPI.lua_type(L, 1) == LuaTypes.LUA_TSTRING)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<char[]>(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)&& translator.Assignable<System.Base64FormattingOptions>(L, 6)) 
                {
                    byte[] inArray = LuaAPI.lua_tobytes(L, 1);
                    int offsetIn = LuaAPI.xlua_tointeger(L, 2);
                    int length = LuaAPI.xlua_tointeger(L, 3);
                    char[] outArray = (char[])translator.GetObject(L, 4, typeof(char[]));
                    int offsetOut = LuaAPI.xlua_tointeger(L, 5);
                    System.Base64FormattingOptions options;translator.Get(L, 6, out options);
                    
                        int __cl_gen_ret = System.Convert.ToBase64CharArray( inArray, offsetIn, length, outArray, offsetOut, options );
                        LuaAPI.xlua_pushinteger(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to System.Convert.ToBase64CharArray!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FromBase64String_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string s = LuaAPI.lua_tostring(L, 1);
                    
                        byte[] __cl_gen_ret = System.Convert.FromBase64String( s );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FromBase64CharArray_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    char[] inArray = (char[])translator.GetObject(L, 1, typeof(char[]));
                    int offset = LuaAPI.xlua_tointeger(L, 2);
                    int length = LuaAPI.xlua_tointeger(L, 3);
                    
                        byte[] __cl_gen_ret = System.Convert.FromBase64CharArray( inArray, offset, length );
                        LuaAPI.lua_pushstring(L, __cl_gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception __gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + __gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
