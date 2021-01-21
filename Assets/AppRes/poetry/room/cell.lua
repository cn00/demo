
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

-- cell

local print = function ( ... )
    _G.print("cell", ...)
    -- _G.print("cell", debug.traceback())
end

local cell = {}
local this = cell

function cell.init(info)
    this.info = info
end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.TableViewCell = gameObject:GetComponent(typeof(CS.TableView.TableViewCell))
    this.description_Text = description:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.hosturl_Text = hosturl:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.id_Text = id:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.name_Text = name:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function cell.Awake()
	this.AutoGenInit()
end

-- function cell.OnEnable() end

function cell.Start()
	--util.coroutine_call(this.coroutine_demo)
    this.id_Text.text = this.info.id
    this.description_Text.text = this.info.description
    this.hosturl_Text.text = this.info.url
    this.name_Text.text = this.info.name
end



return cell
