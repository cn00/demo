-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

local this = {name="SceneManager"}

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

local print = function(...)
    _G.print("[manager/scene.lua]", ...)
end

--[[
    path = GameObject
]]
local loadstack = {}
this.loadstack = loadstack


function this.push(prefabPath, callback)
    util.coroutine_call(function()
        print('scene_manager push -->', prefabPath)

        local last = loadstack[#loadstack]
        if(last ~= nil and last.obj ~= nil)then
            GameObject.DestroyImmediate(last.obj)
            last.obj = nil
        end
        
        -- todo: open loading

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

function this.pop(callback)
    local last = table.remove(loadstack)
    local newlast = table.remove(loadstack)
    this.push(newlast.path, function (go)
        GameObject.DestroyImmediate(last.obj)
        if callback then
            callback(gameObj)
        end
    end)
end

--AutoGenInit Begin
function this.AutoGenInit()
end
--AutoGenInit End

-- function this.Awake()
-- 	this.AutoGenInit()
-- end

-- function this.OnEnable()
--     print("this.OnEnable")

-- end

-- function this.OnDestroy()
--     print("this.OnDestroy")

-- end
    
return this
