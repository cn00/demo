
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/20 18:41:59
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.util"
local xutil = require "xlua.util"

-- room

local print = function ( ... )
    _G.print("cell", ...)
    -- _G.print("cell", debug.traceback())
end

local roomItem = {}
local this = roomItem

function roomItem.init(info)
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
    this.button_Button = button:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.button_Button.onClick:AddListener(this.button_OnClick)
    this.description_Text = description:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.hosturl_Text = hosturl:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.id_Text = id:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.name_Text = name:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.TableViewCell = gameObject:GetComponent(typeof(CS.TableView.TableViewCell))
end
--AutoGenInit End

function this.button_OnClick()
    print('button_OnClick')
    if this.info.onClickRoomCallback then
        this.info.onClickRoomCallback(this.info.roomId)
    end
end -- button_OnClick

function roomItem.Awake()
	this.AutoGenInit()
end

-- function cell.OnEnable() end

function roomItem.Start()
	--util.coroutine_call(this.coroutine_demo)
    this.id_Text.text = this.info.roomId
    this.description_Text.text = this.info.description
    this.hosturl_Text.text = this.info.url
    this.name_Text.text = this.info.masterName
end



return roomItem
