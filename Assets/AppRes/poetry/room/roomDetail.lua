
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/28 18:50:25
--- Description:
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua/utility/util"
local xutil = require "utility.xlua.util"

-- roomDetail

local print = function ( ... )
    _G.print("roomDetail", ...)
    -- _G.print("roomDetail", debug.traceback())
end

local roomDetail = {}
local this = roomDetail



--AutoGenInit Begin
function roomDetail.AutoGenInit() end
--AutoGenInit End

function roomDetail.Awake()
	this.AutoGenInit()
end

-- function roomDetail.OnEnable() end

function roomDetail.Start()
	--util.coroutine_call(this.coroutine_demo)
end



return roomDetail