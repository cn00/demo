
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
local Vector2 = UnityEngine.Vector2
-- create

local print = function ( ... )
    _G.print("create", ...)
    -- _G.print("create", debug.traceback())
end

local this = {
    selectTags = {}
}
local create = this



--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.okBtn_Button = okBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.okBtn_Button.onClick:AddListener(this.okBtn_OnClick)
    this.roomDescription_Text = roomDescription:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.roomName_Text = roomName:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.scrollContent_RectTransform = scrollContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.testContent_Text = testContent:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.testContent_RectTransform = testContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.testRoot_Image = testRoot:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.textBtn_Button = textBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.textBtn_Button.onClick:AddListener(this.textBtn_OnClick)
end
--AutoGenInit End

function this.textBtn_OnClick()
    print('textBtn_OnClick')
    -- refresh ui
    if(not testRoot.activeSelf) then
        -- TODO get poetry from db

        this.testContent_RectTransform.sizeDelta = Vector2(0, this.testContent_Text.preferredHeight)
    end
    testRoot:SetActive(not testRoot.activeSelf)
end -- textBtn_OnClick

function this.BackBtn_OnClick()
    AppGlobal.SceneManager.push("poetry/index/index.prefab", nil, true)
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
    this.testContent_RectTransform.sizeDelta = Vector2(0, this.testContent_Text.preferredHeight)
end

return create
