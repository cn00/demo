-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.xlua.util"

local print = function ( ... )
    _G.print("manager", ... )
    -- _G.print("manager", debug.traceback())
end

local manager = {
    name = "manager",
}
local this = manager
G.manager = manager

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.console_LuaBehaviour = console:GetComponent(typeof(CS.LuaBehaviour))
    this.message_LuaBehaviour = message:GetComponent(typeof(CS.LuaBehaviour))
    this.network_LuaBehaviour = network:GetComponent(typeof(CS.LuaBehaviour))
    this.scene_LuaBehaviour = scene:GetComponent(typeof(CS.LuaBehaviour))
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
    this.Network = this.network_LuaBehaviour.Lua
    this.Scene = this.scene_LuaBehaviour.Lua
    this.Message = this.message_LuaBehaviour.Lua
    this.Console = this.console_LuaBehaviour.Lua

    --assert(coroutine.resume(manager.coroutine_demo()))

end

return manager