
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/12 12:07:03
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

-- index

local print = function ( ... )
    _G.print("[index]", ...)
end

local index = {}
local this = index



--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.bg_Image = bg:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.pvc_Button = pvc:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.pvc_Button.onClick:AddListener(this.pvc_OnClick)
    this.pvp_Button = pvp:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.pvp_Button.onClick:AddListener(this.pvp_OnClick)
end
--AutoGenInit End

function this.pvc_OnClick()
    print('pvc_OnClick')
end -- pvc_OnClick

function this.pvp_OnClick()
    print('pvp_OnClick')
end -- pvp_OnClick

function index.Awake()
	this.AutoGenInit()
end

function index.Start()
	--util.coroutine_call(this.coroutine_demo)
end

return index
