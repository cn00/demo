-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local cell = {}
local self = cell

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function cell.coroutine_demo()
--     return coroutine.create(function()
--         print('cell coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function cell.AutoGenInit()
    cell.InputField_InputField = InputField:GetComponent("UnityEngine.UI.InputField")
end
--AutoGenInit End

function cell.Awake()
	self.AutoGenInit()
end

function cell.OnEnable()
    print("cell.OnEnable")

end

function cell.Start()
    print("cell.Start")

    --assert(coroutine.resume(cell.coroutine_demo()))

end

function cell.FixedUpdate()

end

function cell.Update()

end

function cell.LateUpdate()

end

function cell.OnDestroy()
    print("cell.OnDestroy")

end

function cell.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return cell
