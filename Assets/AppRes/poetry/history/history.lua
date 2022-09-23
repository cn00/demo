-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt
local CS = CS
local UnityEngine = CS.UnityEngine
local Color = UnityEngine.Color
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local File = CS.System.IO.File
local Directory = CS.System.IO.Directory

local print = function ( ... )
    _G.print("poetry/history", ... )
end

local util = require "utility.xlua.util"
local sqlite3 = require("lsqlite3")

local history = {
    DataSource = {}
}
local this = history

local yield_return = util.async_to_sync(function(to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)


--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.BackBtn_Button = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.BackBtn_Button.onClick:AddListener(this.BackBtn_OnClick)
    this.SliderV_Slider = SliderV:GetComponent(typeof(CS.UnityEngine.UI.Slider))
    this.SliderVText_Text = SliderVText:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.tableview_TableView = tableview:GetComponent(typeof(CS.TableView.TableView))
    this.tableview_TableViewController = tableview:GetComponent(typeof(CS.TableView.TableViewController))
end
--AutoGenInit End

function this.BackBtn_OnClick()
    local manager = AppGlobal.manager
    AppGlobal.SceneManager.push("poetry/index/index.prefab", nil, true)
end -- BackBtn_OnClick


function this.Awake()
    this.AutoGenInit()
end

function this.Start()
    this.LoadData() -- TableView:ReloadData() mast be called after Awake
end


function this.LoadData()
    util.coroutine_call(function()
        local records = AppGlobal.Datasys.getuser("history", {"*"})
        this.DataSource = records
        print("records", #records)

        this.InitTableViewData()
        this.tableview_TableView:ReloadData()
    end)
end

function this.InitTableViewData()

    this.tableview_TableViewController.GetDataCount = function(table)
        return #this.DataSource
    end
    this.tableview_TableViewController.GetCellSize = function(table, row)
        local size = 60;
        return size
    end
    this.tableview_TableViewController.CellAtRow = function(tb, row)
        --print("CellAtRow", row)
        local celltypenumber = this.tableview_TableViewController.prefabCells.Length
        local cellidentifier = string.format("cell_%02d", 1+(row % celltypenumber))
        --print("cellidentifier",cellidentifier,row,celltypenumber)
        local cell = tb:ReusableCellForRow(cellidentifier, row)
        cell.name = "lua-Cell-" ..(row)
        local cellmono = cell:GetComponent("LuaBehaviour")
        local ct = cellmono.Lua
        local cdata = this.DataSource[row + 1]
        cdata.row = row+1

        if(cdata.scoreA > cdata.scoreB)then ct.Image_Image.color = Color(0.3, 0.2, 0, 0.2) end

        --local delcallback = function(cbid)
        --    -- row changed  after remove
        --    for i, v in ipairs(this.DataSource) do
        --        if v.id == cbid then
        --            table.remove(this.DataSource, i)
        --        end
        --    end
        --    this.tableview_TableView:ReloadData()
        --end

        ct.init({ data = cdata})

        return cell
    end

    -- this.initData()
    -- this.tableview_TableViewController.tableView:ReloadData()

    this.SliderV_Slider.onValueChanged:AddListener(function(fval)
        -- print("SliderV_Slider.onValueChanged", fval, this.SliderV_Slider.value)
        this.SliderVText_Text.text = string.format("%.0f", fval * 100)
        this.tableview_TableViewController.tableView:SetPosition(fval * this.tableview_TableViewController.tableView.ContentSize)
    end)
end


return history