-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

local print = function ( ... )
    _G.print("[manager]", ... )
    -- _G.print("[manager]", debug.traceback())
end

local manager = {
    name = "manager",
}
local this = manager
G.manager = manager

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.console_LuaMonoBehaviour = console:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.message_LuaMonoBehaviour = message:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.network_LuaMonoBehaviour = network:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.scene_LuaMonoBehaviour = scene:GetComponent(typeof(CS.LuaMonoBehaviour))
end
--AutoGenInit End

function this.Awake()
    this.AutoGenInit()
    
    print("manager.Awake")

end

function this.OnEnable()
    print("manager.OnEnable")
end

function this.Start()
    print("manager.Start")
    this.Network = this.network_LuaMonoBehaviour.Lua
    this.Scene = this.scene_LuaMonoBehaviour.Lua
    this.Message = this.message_LuaMonoBehaviour.Lua
    this.Console = this.console_LuaMonoBehaviour.Lua

    --assert(coroutine.resume(manager.coroutine_demo()))

end
    
return manager
