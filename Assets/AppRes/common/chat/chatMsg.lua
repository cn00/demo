
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
local util = require "utility.util"
local xutil = require "utility.xlua.util"
local Vector2 = UnityEngine.Vector2

local yield_return = xutil.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

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
    this.emoji_Image = emoji:GetComponent(typeof(CS.UnityEngine.UI.Image))
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
        xutil.coroutine_call(function()
            local af = string.format("common/emoji/%d.png", emojiId)
            yield_return(CS.AssetSys.GetAsset(af, function(t2d)
                print("emoji", af, t2d)
                if UNITY_EDITOR then
                    this.emoji_Image.sprite = UnityEngine.Sprite.Create(t2d,
                            this.emoji_Image.sprite.rect,
                            this.emoji_Image.sprite.pivot)
                else
                    this.emoji_Image.sprite = t2d
                end
                this.chatContent_Text.text = ""
                emoji:SetActive(true)
            end))
        end)
    else
        emoji:SetActive(false)
        this.chatContent_Text.text = this.info.content
    end

    --this.RectTransform.sizeDelta = Vector2(0, this.chatContent_Text.rectTransform.sizeDelta.y)
    end

return chatMsg