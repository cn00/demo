-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local excel_test = {}
local self = excel_test

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function excel_test.coroutine_demo()
--     return coroutine.create(function()
--         print('excel_test coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function excel_test.AutoGenInit() end
--AutoGenInit End

function excel_test.Awake()
	self.AutoGenInit()
end

function excel_test.OnEnable()
    print("excel_test.OnEnable")

end

function excel_test.Start()
    print("excel_test.Start")

    --assert(coroutine.resume(excel_test.coroutine_demo()))

end

function excel_test.FixedUpdate()

end

function excel_test.Update()

end

function excel_test.LateUpdate()

end

function excel_test.OnDestroy()
    print("excel_test.OnDestroy")

end

function excel_test.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return excel_test
