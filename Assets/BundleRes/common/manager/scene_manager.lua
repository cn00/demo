-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local scene_manager = {}
local this = scene_manager

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)


--[[
    path = GameObject
]]
local loadstack = {}

function start_coroutine(func)
    assert(coroutine.resume(coroutine.create(func)))
end

function scene_manager.load(prefabPath, callback)
    start_coroutine(function()
        print('scene_manager coroutine start!')

        -- open loading

        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset(prefabPath, function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)
        table.insert(loadstack, {path = prefabPath, obj = gameObj})

        if callback then
        	callback(gameObj)
        end
    end)
end

function scene_manager.pop(callback)
    start_coroutine(function()
        local last = loadstack[#loadstack]
        table.remove(loadstack)

        local secondlast = loadstack[#loadstack]
        table.remove(loadstack)
        yield_return(scene_manager.load(secondlast.path, function(obj)
            GameObject.DestroyImmediate(last.obj)
        end))
    end)
end

--AutoGenInit Begin
function scene_manager.AutoGenInit()
end
--AutoGenInit End

-- function scene_manager.Awake()
-- 	this.AutoGenInit()
-- end

-- function scene_manager.OnEnable()
--     print("scene_manager.OnEnable")

-- end

-- function scene_manager.OnDestroy()
--     print("scene_manager.OnDestroy")

-- end
    
return scene_manager
