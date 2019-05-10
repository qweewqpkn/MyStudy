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
    public class DebugerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Debuger);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 13, 1, 1);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "Init", _m_Init_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "Log", _m_Log_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogWarning", _m_LogWarning_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogError", _m_LogError_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SwitchModule", _m_SwitchModule_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SwitchTarget", _m_SwitchTarget_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SwitchLevel", _m_SwitchLevel_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SwitchBuffer", _m_SwitchBuffer_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "OutputBufferToFile", _m_OutputBufferToFile_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetBuffer", _m_GetBuffer_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "ClearBuffer", _m_ClearBuffer_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "UploadFiles", _m_UploadFiles_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "IsOpenLog", _g_get_IsOpenLog);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "IsOpenLog", _s_set_IsOpenLog);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					Debuger gen_ret = new Debuger();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Debuger constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Init_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1&& translator.Assignable<YYUIWarpContent>(L, 1)) 
                {
                    YYUIWarpContent _content = (YYUIWarpContent)translator.GetObject(L, 1, typeof(YYUIWarpContent));
                    
                    Debuger.Init( _content );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 0) 
                {
                    
                    Debuger.Init(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to Debuger.Init!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Log_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _type = LuaAPI.lua_tostring(L, 1);
                    string _message = LuaAPI.lua_tostring(L, 2);
                    
                    Debuger.Log( _type, _message );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogWarning_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _type = LuaAPI.lua_tostring(L, 1);
                    string _message = LuaAPI.lua_tostring(L, 2);
                    
                    Debuger.LogWarning( _type, _message );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogError_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _type = LuaAPI.lua_tostring(L, 1);
                    string _message = LuaAPI.lua_tostring(L, 2);
                    
                    Debuger.LogError( _type, _message );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SwitchModule_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _type = LuaAPI.lua_tostring(L, 1);
                    bool _isOpen = LuaAPI.lua_toboolean(L, 2);
                    
                    Debuger.SwitchModule( _type, _isOpen );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SwitchTarget_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    Debuger.OutputTarget _target;translator.Get(L, 1, out _target);
                    bool _isOpen = LuaAPI.lua_toboolean(L, 2);
                    
                    Debuger.SwitchTarget( _target, _isOpen );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SwitchLevel_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    Debuger.OutputLevel _level;translator.Get(L, 1, out _level);
                    bool _isOpen = LuaAPI.lua_toboolean(L, 2);
                    
                    Debuger.SwitchLevel( _level, _isOpen );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SwitchBuffer_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _type = LuaAPI.lua_tostring(L, 1);
                    bool _isOpen = LuaAPI.lua_toboolean(L, 2);
                    
                    Debuger.SwitchBuffer( _type, _isOpen );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OutputBufferToFile_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _FileName = LuaAPI.lua_tostring(L, 2);
                    
                    Debuger.OutputBufferToFile( _module, _FileName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetBuffer_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _type = LuaAPI.lua_tostring(L, 1);
                    
                        System.Collections.Generic.List<string> gen_ret = Debuger.GetBuffer( _type );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearBuffer_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _type = LuaAPI.lua_tostring(L, 1);
                    
                    Debuger.ClearBuffer( _type );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UploadFiles_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _playerID = LuaAPI.lua_tostring(L, 1);
                    
                    Debuger.UploadFiles( _playerID );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsOpenLog(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushboolean(L, Debuger.IsOpenLog);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_IsOpenLog(RealStatePtr L)
        {
		    try {
                
			    Debuger.IsOpenLog = LuaAPI.lua_toboolean(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
