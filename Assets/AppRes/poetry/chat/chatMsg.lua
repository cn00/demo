
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/29 21:30:14
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "util"
local xutil = require "xlua.util"

-- chatMsg

local print = function ( ... )
    _G.print("chatMsg", ...)
    -- _G.print("chatMsg", debug.traceback())
end

local chatMsg = {}
local this = chatMsg



--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.chatContent_Text = chatContent:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.userName_Text = userName:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function chatMsg.Awake()
	this.AutoGenInit()
end


return chatMsg
