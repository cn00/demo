-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

local table_manager = {}
local this = table_manager

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function table_manager.coroutine_demo()
--     return coroutine.create(function()
--         print('table_manager coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function table_manager.AutoGenInit() end
--AutoGenInit End

function table_manager.Awake()
	this.AutoGenInit()
end

function table_manager.OnEnable()
    print("table_manager.OnEnable")

end

function table_manager.Start()
    print("table_manager.Start")

    --assert(coroutine.resume(table_manager.coroutine_demo()))

end

function table_manager.FixedUpdate()

end

function table_manager.Update()

end

function table_manager.LateUpdate()

end

function table_manager.OnDestroy()
    print("table_manager.OnDestroy")

end
    
return table_manager
