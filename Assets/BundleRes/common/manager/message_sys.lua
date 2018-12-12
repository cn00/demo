-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local message_sys = {}
local this = message_sys

local msgs = {
    -- key = {listeners}
}
this.msgs = msgs

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function message_sys.coroutine_demo()
--     return coroutine.create(function()
--         print('message_sys coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function this.AutoGenInit()
    this.message_sys_LuaMonoBehaviour = message_sys:GetComponent("LuaMonoBehaviour")
    this.scene_manager_LuaMonoBehaviour = scene_manager:GetComponent("LuaMonoBehaviour")
    this.network_LuaMonoBehaviour = network:GetComponent("LuaMonoBehaviour")
    this.console_LuaMonoBehaviour = console:GetComponent("LuaMonoBehaviour")
end
--AutoGenInit End

function message_sys.Awake()
end

function message_sys.OnEnable()
    print("message_sys.OnEnable")

end

function message_sys.Start()
	this.AutoGenInit()
    print("message_sys.Start")

    --assert(coroutine.resume(message_sys.coroutine_demo()))

end

function message_sys.FixedUpdate()

end

function message_sys.Trigger( key, data )
    for k, v in pairs(msgs[key]) do
        v(data)
    end
    msgs[key] = nil
end

function message_sys.AddListener( key, fun )
    local event = msgs[key] or {}
    if event[fun] == nil then
        event[fun] = fun
    end
    msgs[key] = event
end

function message_sys.Update()
end

function message_sys.LateUpdate()

end

function message_sys.OnDestroy()
    print("message_sys.OnDestroy")

end
    
return message_sys
