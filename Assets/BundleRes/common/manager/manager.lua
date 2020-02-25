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

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function manager.coroutine_demo()
--     return coroutine.create(function()
--         print('manager coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.message_LuaMonoBehaviour = message:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.scene_LuaMonoBehaviour = scene:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.network_LuaMonoBehaviour = network:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.console_LuaMonoBehaviour = console:GetComponent(typeof(CS.LuaMonoBehaviour))
    this.Button_Button = Button:GetComponent(typeof(CS.UnityEngine.UI.Button))
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
    this.network = this.network_LuaMonoBehaviour.Lua
    this.scene = this.scene_LuaMonoBehaviour.Lua
    this.message = this.message_LuaMonoBehaviour.Lua
    this.console = this.console_LuaMonoBehaviour.Lua
    print("manager"
        ,this.network
        ,this.scene
        ,this.message
        ,this.console
    )

    --assert(coroutine.resume(manager.coroutine_demo()))

end

function this.FixedUpdate()

end

function this.Update()

end

function this.LateUpdate()

end

function this.OnDestroy()
    print("manager.OnDestroy")

end
    
return manager
