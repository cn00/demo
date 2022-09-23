-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite3 = require("lsqlite3")
local manager = manager

local print = function ( ... )
    _G.print("history.cell", ... )
end

local util = require "utility.xlua.util"

local cell = {
    data = nil, --{id, na, a, nb, b, date}
    delcallback = nil,
}
local this = cell
local self = cell

local yield_return = util.async_to_sync(function (to_yield, callback)
     mono:YieldAndCallback(to_yield, callback)
end)

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.TableViewCell = gameObject:GetComponent(typeof(CS.TableView.TableViewCell))
    this.Animator = gameObject:GetComponent(typeof(CS.UnityEngine.Animator))
    this.date_Text = date:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.DelBtn_Button = DelBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.DelBtn_Button.onClick:AddListener(this.DelBtn_OnClick)
    this.dt_Text = dt:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.id_Text = id:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Image_Image = Image:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.na_Text = na:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.nb_Text = nb:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.va_Text = va:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.vb_Text = vb:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function this.init(info)
    this.data = info.data
    this.delcallback = info.delcallback
end

function this.DelBtn_OnClick()
    local dbpath = AssetSys.CacheRoot .. "db.db"
    local db = sqlite3.open(dbpath)
    local errn = db:exec("delete from history where id = " .. this.data.id .. ";")
    if errn ~= sqlite3.OK then print("delete history err", db:errmsg())end
    db:close()
    if this.delcallback then this.delcallback(this.data.id) end
end -- DelBtn_OnClick

function this.Awake()
    this.AutoGenInit()
end

function this.Start()
    --{id, na, a, nb, b, date}
    local data = this.data
    if data then
        this.id_Text.text   = data.row
        this.na_Text.text   = data.nameA
        this.va_Text.text   = data.scoreA
        this.dt_Text.text   = string.format("%.1f", data.useTime)
        this.nb_Text.text   = data.nameB
        this.vb_Text.text   = data.scoreB
        this.date_Text.text = data.date
    end

    local ct = cell
    ct.TableViewCell:DidSelectEvent("+", function(row2)
        print("cellevent DidSelectEvent", row2, ct.TableViewthis.RowNumber)
    end)
    ct.TableViewCell:DidPointClickEvent("+", function(row2)
        print("cellevent DidPointClickEvent", row2, this.RowNumber, ct.Animator, ct.Animator.Play)
        ct.Animator:Play("cell_scale", 0)
        --this.Play()
    end)
    ct.TableViewCell:DidHighlightEvent("+", function(row2)
        print("cellevent DidHighlightEvent", row2, this.RowNumber)
    end)
end

return cell