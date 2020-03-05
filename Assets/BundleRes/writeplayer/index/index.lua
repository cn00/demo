-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt
local CS = CS
local UnityEngine = CS.UnityEngine
local Color = UnityEngine.Color
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local File = CS.System.IO.File
local Directory = CS.System.IO.Directory

local sqlite3 = require("lsqlite3")

local print = function ( ... )
    _G.print("[writeplayer.index]", ... )
end

local util = require "lua.utility.xlua.util"

local excel_view = {
    RowIdxA = 1,

    ColumnIdxA = 0,
    ColumnPerPage = 5,
    ColumnPerPageMax = 7,

    DataSource = {}
}
local this = excel_view

local yield_return = util.async_to_sync(function(to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)


--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.tableview_TableView = tableview:GetComponent(typeof(CS.TableView.TableView))
    this.tableview_TableViewController = tableview:GetComponent(typeof(CS.TableView.TableViewController))
    this.grep_Button = grep:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.reset_Button = reset:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.back_Button = back:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.SliderV_Slider = SliderV:GetComponent(typeof(CS.UnityEngine.UI.Slider))
    this.SliderVText_Text = SliderVText:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function string.split(s, delimiter)
    local result = {};
    for match in (s..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match);
    end
    return result;
end
function this.Awake()
    print("Awake-0")
    this.AutoGenInit()
    print("Awake-1")
    
    --xlua.private_accessible(CS.TableView.TableView)

    util.coroutine_call(this.LoadData)
    
end


function this.LoadData()
    local dburl = "res/db.db"
    local cachePath = AssetSys.CacheRoot .. "db.db"
    if not File.Exists(cachePath) then
        yield_return(AssetSys.Download(dburl, cachePath))
    else
        print("use cache:", cachePath)
    end
    
    yield_return(AssetSys.Instance:GetBundle("font/fzxz.bd"))

    local sql
    yield_return(AssetSys.Instance:GetAsset("writeplayer/index/init.sql", function(asset)
        sql = asset.text
        print("init.sql", sql)
    end))

    local db = sqlite3.open(cachePath);
    db:exec(sql)
    
    yield_return(AssetSys.Instance:GetAsset("writeplayer/index/itemlist.sql", function(asset)
        sql = asset.text
    end))
    local r
    local vm = db:prepare(sql)
    assert(vm, db:errmsg())
    print('====================================')
    print(vm:get_unames())
    print('------------------------------------')
    local head = table.pack(vm:get_unames())
    --for i = 1,#head do head[head[i]] = i end
    local records = {head = head}
    --local mt = {
    --    __newindex = function(t, k, v)
    --        print("__newindex", k, t[k], v)
    --        if t[k] == nil then
    --            return t[head[k]]
    --        end
    --    end,
    --}
    r = vm:step()
    while (r == sqlite3.ROW) do
        local record = table.pack(vm:get_uvalues())
        records[#records + 1] = {
            id = record[1],
            name = record[2],
            url = record[3],
            cpath = record[4],
            tpath = record[5],
            text = record[6],
        }
        --setmetatable(record,mt)
        r = vm:step()
    end
    assert(r == sqlite3.DONE)
    assert(vm:finalize() == sqlite3.OK)
    print('====================================')
    this.DataSource = records
    
    db:close()
    
    this.InitTableViewData()
    this.tableview_TableView:ReloadData()
end

function this.InitTableViewData()
        
    this.tableview_TableViewController.GetDataCount = function(table)
        return #this.DataSource
    end
    this.tableview_TableViewController.GetCellSize = function(table, row)
        
        -- print("GetCellSize: "..row)
        local size = 160;
        --local hightperline = 40
        --local excel_row = this.DataSource[row + 1]
        --for i = this.ColumnIdxA, this.ColumnIdxA + this.ColumnPerPage do
        --    local cell = excel_row:GetCell(i)
        --    if cell ~= nil then
        --        local s = cell:StrValue()
        --        local ss = s:split('\n')
        --        local lc = 1
        --        for i,v in ipairs(ss) do
        --            lc = lc + 1 + math.ceil(#v / 3 * 40 /(1700 / this.ColumnPerPage))
        --        end
        --        local tmp = lc * hightperline
        --        if tmp > size then size = tmp end
        --    end
        --end
        
        -- if table.tableView.LayoutOrientation == 1 then
        --     size = cell.rectTransform.height
        -- else
        --     size = cell.rectTransform.width
        -- end
        return size
    end
    this.tableview_TableViewController.CellAtRow = function(tb, row)
        this.SliderV_Slider.value = this.tableview_TableViewController.tableView.Position / this.tableview_TableViewController.tableView.ContentSize
        local celltypenumber = this.tableview_TableViewController.prefabCells.Length
        local cellidentifier = string.format("cell_%02d", 1+(row % celltypenumber))
        print("cellidentifier",cellidentifier,row,celltypenumber)
        local cell = tb:ReusableCellForRow(cellidentifier, row)
        cell.name = "lua-Cell-" ..(row)
        local cellmono = cell:GetComponent("LuaMonoBehaviour")
        local ct = cellmono.Lua
        ct.SetCellData(this.DataSource[row + 1], this.ColumnIdxA, this.ColumnPerPage)
        if(row%2 == 0)then ct.Image_Image.color = Color(0.3, 0.4, 0, 0.2) end

        -- print("CellAtRow", row)
        return cell
    end
    
    -- this.initData()
    -- this.tableview_TableViewController.tableView:ReloadData()
    this.reset_Button.onClick:AddListener(function()
        -- this.initData()
        this.ColumnIdxA = this.ColumnIdxA + 1
        if this.ColumnIdxA > 10 then
            this.ColumnIdxA = 10
        end
        this.Slider_Slider.value = this.ColumnIdxA / 10
        
        this.tableview_TableViewController.tableView:ReloadData()
    end)
    
    this.grep_Button.onClick:AddListener(util.coroutine_call(function()
         local obj = nil
         yield_return(CS.AssetSys.Instance:GetAsset("writeplayer/itemimport/itemimport.prefab", function(asset)
             obj = asset
         end))
         local gameObj = GameObject.Instantiate(obj)
    end))
    this.SliderV_Slider.onValueChanged:AddListener(function(fval)
        -- print("SliderV_Slider.onValueChanged", fval, this.SliderV_Slider.value)
        this.SliderVText_Text.text = string.format("%.0f", fval * 100)
        this.tableview_TableViewController.tableView:SetPosition(fval * this.tableview_TableViewController.tableView.ContentSize)
    end)
    
    this.back_Button.onClick:AddListener(function()
        this.Back()
    end)
end

function this.Back()
    assert(coroutine.resume(coroutine.create(function()
        yield_return(UnityEngine.WaitForSeconds(0.3))
        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("ui/test/test.prefab", function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)
        
        GameObject.DestroyImmediate(mono.gameObject)
    end)))
end

function this.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return excel_view
