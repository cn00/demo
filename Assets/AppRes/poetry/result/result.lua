
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/14 18:04:51
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

-- result

local print = function ( ... )
    _G.print("[result]", ...)
end

local result = {}
local this = result



--AutoGenInit Begin
function result.AutoGenInit() end
--AutoGenInit End

function result.Awake()
	this.AutoGenInit()
end

-- function result.OnEnable() end

function result.Start()
	--util.coroutine_call(this.coroutine_demo)
end



function result.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return result
