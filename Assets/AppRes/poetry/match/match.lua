
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/08 19:18:14
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

-- match

local print = function ( ... )
    _G.print("[match]", ...)
end

local match = {}
local this = match



--AutoGenInit Begin
function match.AutoGenInit() end
--AutoGenInit End

function match.Awake()
	this.AutoGenInit()
end

-- function match.OnEnable() end

function match.Start()
	--util.coroutine_call(this.coroutine_demo)
end



function match.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return match
