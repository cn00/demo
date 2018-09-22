-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local manager = {
    name = "manager",
    G = G
}
local self = manager
_G.manager = manager
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
function manager.AutoGenInit()
end
--AutoGenInit End

function manager.Awake()
	self.AutoGenInit()
end

function manager.OnEnable()
    print("manager.OnEnable")

end

function manager.Start()
    print("manager.Start")

    --assert(coroutine.resume(manager.coroutine_demo()))

end

function manager.FixedUpdate()

end

function manager.Update()

end

function manager.LateUpdate()

end

function manager.OnDestroy()
    print("manager.OnDestroy")

end
    
return manager
