
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/02/18 19:24:15
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

-- chat

local print = function ( ... )
    _G.print("chat", ...)
    -- _G.print("chat", debug.traceback())
end

local this = {
    chatMsgNew = false,
    chatHistory = {},
}
local chat = this



--AutoGenInit Begin
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.ChatBtn_Button = ChatBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.ChatBtn_Button.onClick:AddListener(this.ChatBtn_OnClick)
    this.chatContent_RectTransform = chatContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.chatInputField_InputField = chatInputField:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.chatMsgTemp_Image = chatMsgTemp:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.chatSendBtn_Button = chatSendBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.chatSendBtn_Button.onClick:AddListener(this.chatSendBtn_OnClick)
    this.emojiBtn_Button = emojiBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.emojiBtn_Button.onClick:AddListener(this.emojiBtn_OnClick)
    this.emojiContent_RectTransform = emojiContent:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.emojiSV_RectTransform = emojiSV:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.emojiTemp_RectTransform = emojiTemp:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.uiroot_RectTransform = uiroot:GetComponent(typeof(CS.UnityEngine.RectTransform))
end
--AutoGenInit End

function this.ChatBtn_OnClick()
    uiroot:SetActive(not uiroot.activeSelf)
    print('ChatBtn_OnClick')
end -- ChatBtn_OnClick

function this.chatSendBtn_OnClick()
    print('chatSendBtn_OnClick')
    if this.chatInputField_InputField.text == "" then return end
    AppGlobal.Client.SendMsgt({
        type = "chat",
        clientId = this.clientId,
        roomId = this.roomId,
        content = this.chatInputField_InputField.text
    })
    this.chatInputField_InputField.text = ""
end -- chatSendBtn_OnClick


-- refresh msg ui
local function refreshChat()
    if gameObject.activeSelf then
        if this.chatMsgNew then
            for i, v in ipairs(this.chatHistory) do
                if v.obj == nil then
                    local obj = GameObject.Instantiate(chatMsgTemp, this.chatContent_RectTransform)
                    obj:SetActive(true)
                    v.obj = obj
                    local com = obj:GetComponent(typeof(CS.LuaBehaviour)).Lua
                    com.init(v)
                end
            end
            this.chatMsgNew = false
        end
    end
end

function this.emojiBtn_OnClick()
    print('emojiBtn_OnClick')
    emojiSV:SetActive(not emojiSV.activeSelf)
end -- emojiBtn_OnClick

function chat.Awake()
	this.AutoGenInit()
end

-- function chat.OnEnable() end

function chat.Start()
	--util.coroutine_call(this.coroutine_demo)
    chatMsgTemp:SetActive(false)
    emojiTemp:SetActive(false)

    xutil.coroutine_call(function()
        -- init emoji
        local total = 52
        for i = 1, total do
            local item = GameObject.Instantiate(emojiTemp, this.emojiContent_RectTransform)
            item:SetActive(true)
            item.name = "emoji_" .. i
            local com = item:GetComponent(typeof(CS.LuaBehaviour)).Lua
            if com then com.init({id = i, onClick = function(id)
                AppGlobal.Client.SendMsgt({
                    type = "chat",
                    clientId = this.clientId,
                    roomId = this.roomId,
                    content = string.format("{emoji:%d}", id)
                })
                emojiSV:SetActive(not emojiSV.activeSelf)
            end}) end
        end
        this.emojiContent_RectTransform.sizeDelta = Vector2(0, 54 * ((total+4)//8))
        emojiSV:SetActive(false)
    end)

    uiroot:SetActive(false)
    AppGlobal.Client.AddListeners(this.OnServerMsgListeners)
end


--- body {clientId, content}
local function OnChat(msgt)
    table.insert(this.chatHistory, msgt)
    this.chatMsgNew = true
    refreshChat()
    return true
end
chat.OnServerMsgListeners = {
    ["chat"]		= OnChat,
}

return chat