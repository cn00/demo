
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/02/01 15:05:59
--- Description:
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.util"
local xutil = require "utility.xlua.util"

-- poetryutil

local print = function ( ... )
    _G.print("poetryutil", ...)
    -- _G.print("poetryutil", debug.traceback())
end

local this = {}
local poetryutil = this



--AutoGenInit Begin
function poetryutil.AutoGenInit() end
--AutoGenInit End

function poetryutil.Awake()
	this.AutoGenInit()
end

-- function poetryutil.OnEnable() end

function poetryutil.Start()
	--util.coroutine_call(this.coroutine_demo)
end



return poetryutil