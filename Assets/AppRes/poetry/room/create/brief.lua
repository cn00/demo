
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/20 16:06:11
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "xlua.util"

-- brief

local print = function ( ... )
    _G.print("brief", ...)
    -- _G.print("brief", debug.traceback())
end

local brief = {}
local this = brief



--AutoGenInit Begin
function brief.AutoGenInit() end
--AutoGenInit End

function brief.Awake()
	this.AutoGenInit()
end

-- function brief.OnEnable() end

function brief.Start()
	--util.coroutine_call(this.coroutine_demo)
end



return brief
