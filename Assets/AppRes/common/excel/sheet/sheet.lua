-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.xlua.util"

local sheet = {}
local self = sheet

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function sheet.coroutine_demo()
--     return coroutine.create(function()
--         print('sheet coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function sheet.AutoGenInit() end
--AutoGenInit End

function sheet.Awake()
	self.AutoGenInit()
end

function sheet.OnEnable()
    print("sheet.OnEnable")

end

function sheet.Start()
    print("sheet.Start")

    --assert(coroutine.resume(sheet.coroutine_demo()))

end

function sheet.FixedUpdate()

end

function sheet.Update()

end

function sheet.LateUpdate()

end

function sheet.OnDestroy()
    print("sheet.OnDestroy")

end

function sheet.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return sheet