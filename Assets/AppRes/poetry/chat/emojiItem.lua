
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/02/18 19:59:03
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "util"
local xutil = require "xlua.util"
local AssetSys = CS.AssetSys

-- emojiItem

local print = function ( ... )
    _G.print("emojiItem", ...)
    -- _G.print("emojiItem", debug.traceback())
end

local this = {}
local emojiItem = this

function emojiItem.init(info)
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
    this.RectTransform = gameObject:GetComponent(typeof(CS.UnityEngine.RectTransform))
    this.RawImage = gameObject:GetComponent(typeof(CS.UnityEngine.UI.RawImage))
    this.Button = gameObject:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.Button.onClick:AddListener(this.emojiTemp_OnClick)
end
--AutoGenInit End

function this.emojiTemp_OnClick()
    print('emojiTemp_OnClick')
    if this.info.onClick then
        this.info.onClick(this.info.id)
    end
end -- emojiTemp_OnClick


function emojiItem.Awake()
	this.AutoGenInit()
end

-- function emojiItem.OnEnable() end

function emojiItem.Start()
     xutil.coroutine_call(function()
         local af = string.format("common/emoji/%d.png", this.info.id)
         local tx = AssetSys.GetAssetSync(af)
         print("af", af, tx)
         this.RawImage.texture = tx
     end)
end



return emojiItem
