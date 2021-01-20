
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/20 15:44:34
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "util"
local xutil = require "xlua.util"
local manager = AppGlobal.manager

-- create

local print = function ( ... )
    _G.print("[create]", ...)
    -- _G.print("[create]", debug.traceback())
end

local create = {}
local this = create



--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.okBtn_Button = okBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.okBtn_Button.onClick:AddListener(this.okBtn_OnClick)
    this.roomDescription_Text = roomDescription:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.roomName_Text = roomName:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.scrollContent_RectTransform = scrollContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
end
--AutoGenInit End

function this.BackBtn_OnClick()
    manager.Scene.push("poetry/index/index.prefab", nil, true)
end -- BackBtn_OnClick

function this.okBtn_OnClick()
    print('okBtn_OnClick')
end -- okBtn_OnClick

function create.Awake()
	this.AutoGenInit()
end

-- function create.OnEnable() end

function create.Start()
	--util.coroutine_call(this.coroutine_demo)
end

return create
