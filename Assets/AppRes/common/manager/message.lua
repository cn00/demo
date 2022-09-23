-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.xlua.util"

local message_sys = {}
local this = message_sys

local msgs = {
    -- key = {listeners}
}
this.msgs = msgs

--AutoGenInit Begin
function this.AutoGenInit()
end
--AutoGenInit End

function message_sys.Awake()
	this.AutoGenInit()
end

function message_sys.Trigger( key, data )
    for k, v in pairs(msgs[key]) do
        if type(v) == "function" then
             v(data)
        else
            print("no a function " .. k .. ":" .. tostring(v))
            msgs[key] = nil
        end
    end
    msgs[key] = nil
end

function message_sys.AddListener( key, fun )
    local event = msgs[key] or {}
    if event[fun] == nil then
        event[fun] = fun
    end
    msgs[key] = event
end

return message_sys