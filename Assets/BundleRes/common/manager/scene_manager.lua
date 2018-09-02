-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local scene_manager = {}
local self = scene_manager

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)


--[[
    path = GameObject
]]
local loaded_scene = {}

function scene_manager.load(prefab, callback)
    return coroutine.create(function()
        print('scene_manager coroutine start!')

        -- open loading

        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset(prefab, function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)

        if callback then
        	callback(gameObj)
        end
    end)
end

--AutoGenInit Begin
function scene_manager.AutoGenInit() end
--AutoGenInit End

function scene_manager.Awake()
	self.AutoGenInit()
end

function scene_manager.OnEnable()
    print("scene_manager.OnEnable")

end

function scene_manager.OnDestroy()
    print("scene_manager.OnDestroy")

end
    
return scene_manager
