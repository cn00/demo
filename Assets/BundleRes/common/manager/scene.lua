-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

local this = {name="SceneManager"}

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)


--[[
    path = GameObject
]]
local loadstack = {}


function this.push(prefabPath, callback)
    print('scene_manager push', prefabPath)
    util.coroutine_call(function(cb)
        print('scene_manager push -->', prefabPath)

        -- todo: open loading

        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset(prefabPath, function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)
        table.insert(loadstack, {path = prefabPath, obj = gameObj})

        if cb then
        	cb(gameObj)
        end
    end, callback)
end

function this.pop(callback)
    return util.coroutine_call(function()
        local last = table.remove(loadstack)

        local newlast = loadstack[#loadstack]
        yield_return(this.load(newlast.path, function(obj)
            GameObject.DestroyImmediate(last.obj)
        end))
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
