-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local message_sys = {}
local self = message_sys

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
function message_sys.AutoGenInit()
end
--AutoGenInit End

function message_sys.Awake()
	self.AutoGenInit()
end

function message_sys.OnEnable()
    print("message_sys.OnEnable")

end

function message_sys.Start()
    print("message_sys.Start")

    --assert(coroutine.resume(message_sys.coroutine_demo()))

end

function message_sys.FixedUpdate()

end

function message_sys.Update()

end

function message_sys.LateUpdate()

end

function message_sys.OnDestroy()
    print("message_sys.OnDestroy")

end
    
return message_sys
