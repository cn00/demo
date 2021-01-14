
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/13 13:01:28
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

-- server

local print = function ( ... )
    _G.print("[server]", ...)
end

local server = {}
local this = server



--AutoGenInit Begin
function server.AutoGenInit() end
--AutoGenInit End

function server.Awake()
	this.AutoGenInit()
end

-- function server.OnEnable() end

function server.Start()
	--util.coroutine_call(this.coroutine_demo)
end



function server.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return server
