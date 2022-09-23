
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/02/05 19:23:41
--- Description:
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua/utility/util"
local xutil = require "utility.xlua.util"

-- loading

local print = function ( ... )
    _G.print("loading", ...)
    -- _G.print("loading", debug.traceback())
end

local this = {}
local loading = this



--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.Text_Text = Text:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function loading.Awake()
	this.AutoGenInit()
end

-- function loading.OnEnable() end

function loading.Start()
	--util.coroutine_call(this.coroutine_demo)
end



return loading