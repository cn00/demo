
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/20 18:40:19
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "xlua.util"

-- roomList

local print = function ( ... )
    _G.print("[roomList]", ...)
    -- _G.print("[roomList]", debug.traceback())
end

local roomList = {}
local this = roomList



--AutoGenInit Begin
function roomList.AutoGenInit() end
--AutoGenInit End

function roomList.Awake()
	this.AutoGenInit()
end

-- function roomList.OnEnable() end

function roomList.Start()
	--util.coroutine_call(this.coroutine_demo)
end



return roomList
