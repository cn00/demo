
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
local util = require "util"
local xutil = require "xlua.util"
local manager = G.AppGlobal.manager

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
    this.joinGame_Button = joinGame:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.joinGame_Button.onClick:AddListener(this.joinGame_OnClick)
    this.newGame_Button = newGame:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.newGame_Button.onClick:AddListener(this.newGame_OnClick)
    this.uiroot_RectTransform = uiroot:GetComponent(typeof(CS.UnityEngine.RectTransform))
end
--AutoGenInit End

function index.Awake()
    this.AutoGenInit()
end

function index.Start()
    --util.coroutine_call(this.coroutine_demo)
end

function this.joinGame_OnClick()
    manager.Scene.push("common/qrcode/qrcode.prefab", function(url)
        local args = {
            tp = 1, -- 0:主场， 1:客场, 2:观众, 暂定为客场，建立连接后再判定是否为观众
        }
        manager.Scene.push("poetry/match/match.prefab", args, true)
    end, false)
end -- joinGame_OnClick

function this.newGame_OnClick()
    uiroot:SetActive(false)
    manager.Scene.push("poetry/match/match.prefab", {
        tp = 0
    })
end -- newGame_OnClick

return index
