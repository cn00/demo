
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/02/01 14:16:45
--- Description:
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua/utility/util"
local xutil = require "utility.xlua.util"

-- tagItem

local print = function ( ... )
    _G.print("tagItem", ...)
    -- _G.print("tagItem", debug.traceback())
end

local this = {
    info = {}, -- {id, text, clickCallback}
}
local tagItem = this

function tagItem.init(info)
    this.info = info
end

--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.mark_Image = mark:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.Button = gameObject:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.Button.onClick:AddListener(this.tagItem_OnClick)
    this.Text_TextVirtical = Text:GetComponent(typeof(CS.TextVirtical))
end
--AutoGenInit End

function this.tagItem_OnClick()
    print('tagItem_OnClick')
    if type(this.info.clickCallback) == "function" then
        if (this.info.clickCallback(this.Text_TextVirtical.text, not mark.activeSelf, this)) then
            mark:SetActive(not mark.activeSelf)
        end
    end
end -- tagItem_OnClick

function tagItem.Awake()
	this.AutoGenInit()
end

-- function tagItem.OnEnable() end

function tagItem.Start()
	--util.coroutine_call(this.coroutine_demo)
    mark:SetActive(false)
    if this.info.text then
        this.Text_TextVirtical.text = this.info.text
    end
end



return tagItem