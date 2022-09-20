-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

local global = {}
local self = global

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function global.coroutine_demo()
--     return coroutine.create(function()
--         print('global coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function global.AutoGenInit()
    global.manager_LuaBehaviour = manager:GetComponent("LuaBehaviour")
    global.scene_manager_LuaBehaviour = scene_manager:GetComponent("LuaBehaviour")
    global.manager_sys_LuaBehaviour = manager_sys:GetComponent("LuaBehaviour")
end
--AutoGenInit End

function global.Awake()
	self.AutoGenInit()
end

function global.OnEnable()
    print("global.OnEnable")

end

function global.Start()
    print("global.Start")

    --assert(coroutine.resume(global.coroutine_demo()))

end

function global.FixedUpdate()

end

function global.Update()

end

function global.LateUpdate()

end

function global.OnDestroy()
    print("global.OnDestroy")

end

function global.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return global