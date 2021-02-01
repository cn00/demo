-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local xutil = require "xlua.util"
local AssetSys = CS.AssetSys

local Scene = { 
    layer = {}, -- front, middle, back
}

local AppGlobal = _G.AppGlobal or {}
local manager = AppGlobal.manager or {}
if manager.Scene == nil then
    manager.Scene = Scene
else
    return
end

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

local print = function(...)
    _G.print("manager/scene.lua", ...)
end

--[[
    path = GameObject
]]
local loadstack = {}
Scene.loadstack = loadstack

Scene.loading = nil
function Scene.openloading()
    if(Scene.loading ~= nil)then
        Scene.loading.go:SetActive(true)
    end
end

function Scene.closeloading()
    if (Scene.loading ~= nil) then
        Scene.loading.go:SetActive(false)
        LoadingValue = 0
        LoadingString = "..."
    end
end


---push
---@param prefabPath string
---@param arg table|function 初始化信息或加载后回调
---@param replace boolean 是否替换当前场景, 默认 false
function Scene.push(prefabPath, arg, replace)

    arg = arg or {}
    --replace = replace or replace == nil
    xutil.coroutine_call(function()
        print('scene_manager push -->', prefabPath)

        local last = loadstack[#loadstack]
        if(replace and last ~= nil and last.obj ~= nil)then
            local com = last.obj:GetComponent(typeof(CS.LuaMonoBehaviour))
            if com and com.Lua and type(com.Lua.stateToSave) == "function" then
                last.savedState = com.Lua.stateToSave()
            end
            GameObject.DestroyImmediate(last.obj)
            last.obj = undef
        end
        
        -- TODO: open loading

        local obj = nil
        yield_return(CS.AssetSys.GetAsset(prefabPath, function(asset)
            obj = asset
        end))
        
        local parent = Scene.layer.middle
        local callback
        if type(arg) == "function" then callback = arg end
        if type(arg) == "table" then parent = arg.parent or parent; callback = arg.callback end
        local gameObj = GameObject.Instantiate(obj, parent)
        local com = gameObj:GetComponent(typeof(CS.LuaMonoBehaviour))
        if com and com.Lua and type(com.Lua.init) == "function" then
            com.Lua.init(arg)
        end
        table.insert(loadstack, {path = prefabPath, obj = gameObj, savedState = arg})
        if callback then callback() end

    end)
end

function Scene.pop(callback)
    local last = table.remove(loadstack)
    local newlast = table.remove(loadstack)
    Scene.push(newlast.path, function (go)
        GameObject.DestroyImmediate(last.obj)
        if callback then
            callback(newlast.savedState)
        end
    end)
end

--AutoGenInit Begin
function Scene.AutoGenInit()
end
--AutoGenInit End

function Scene.Awake()
    --this.AutoGenInit()
    xutil.coroutine_call(function()
        print('scene_manager push -->', prefabPath)

        if Scene.loading == nil then
            local obj
            print("sync_get_asset", obj)
            yield_return(AssetSys.GetAsset("ui/loading/loading.prefab", function(asset)
                obj = asset
            end))
            local go = GameObject.Instantiate(obj)
            go:SetActive(false)
            local lua = go:GetComponent(typeof(CS.LuaMonoBehaviour)).Lua
            Scene.loading = {
                go = go,
                lua = lua
            }
        end
    end)
end

-- function this.OnEnable()
--     print("this.OnEnable")
-- end

-- function this.OnDestroy()
--     print("this.OnDestroy")

-- end
    
return Scene
