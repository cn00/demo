-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local NewLuaScript = {}
local self = NewLuaScript

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function NewLuaScript.coroutine_demo()
--     return coroutine.create(function()
--         print('NewLuaScript coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function NewLuaScript.AutoGenInit() end
--AutoGenInit End

function NewLuaScript.Awake()
	self.AutoGenInit()
end

function NewLuaScript.OnEnable()
    print("NewLuaScript.OnEnable")

end

function NewLuaScript.Start()
    print("NewLuaScript.Start")

    --assert(coroutine.resume(NewLuaScript.coroutine_demo()))

end

function NewLuaScript.FixedUpdate()

end

function NewLuaScript.Update()

end

function NewLuaScript.LateUpdate()

end

function NewLuaScript.OnDestroy()
    print("NewLuaScript.OnDestroy")

end

function NewLuaScript.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

function NewLuaScript.Enter()

end

function NewLuaScript.Back()

end

return NewLuaScript
