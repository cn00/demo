
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
local Vector2 = UnityEngine.Vector2

-- chatMsg

local print = function ( ... )
    _G.print("chatMsg", ...)
    -- _G.print("chatMsg", debug.traceback())
end

local chatMsg = {}
local this = chatMsg

function chatMsg.init(info)
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
    this.chatContent_Text = chatContent:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.emoji_RawImage = emoji:GetComponent(typeof(CS.UnityEngine.UI.RawImage))
    this.userName_Text = userName:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function chatMsg.Awake()
	this.AutoGenInit()
end

function chatMsg.Start()
    this.userName_Text.text = "client_" .. this.info.clientId
    local emojiId = string.match(this.info.content, "{emoji:(%d+)}")
    if emojiId then
        local af = string.format("common/emoji/%d.png", emojiId)
        local tx = CS.AssetSys.GetAssetSync(af)
        this.emoji_RawImage.texture =  tx
        this.chatContent_Text.text = ""
        emoji:SetActive(true)
    else
        emoji:SetActive(false)
        this.chatContent_Text.text = this.info.content
    end

        --this.RectTransform.sizeDelta = Vector2(0, this.chatContent_Text.rectTransform.sizeDelta.y)
    end

return chatMsg
