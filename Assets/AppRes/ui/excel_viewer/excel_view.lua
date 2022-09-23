-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local xutil = require "utility.xlua.util"

local excel_view = {
    RowIdxA = 1,

    ColumnIdxA = 0,
    ColumnPerPage = 5,
    ColumnPerPageMax = 7,

    dataSource = {}
}
local this = excel_view

local yield_return = xutil.async_to_sync(function(to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- function this.coroutine_demo()
--     return coroutine.create(function()
--         print('excel_view coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end
--AutoGenInit Begin
function this.AutoGenInit()
    this.tableview_TableView = tableview:GetComponent("TableView.TableView")
    this.tableview_TableViewController = tableview:GetComponent("TableView.TableViewController")
    this.grep_Button = grep:GetComponent("UnityEngine.UI.Button")
    this.reset_Button = reset:GetComponent("UnityEngine.UI.Button")
    this.count_Text = count:GetComponent("UnityEngine.UI.Text")
    this.back_Button = back:GetComponent("UnityEngine.UI.Button")
    this.Slider_Slider = Slider:GetComponent("UnityEngine.UI.Slider")
    this.SliderV_Slider = SliderV:GetComponent("UnityEngine.UI.Slider")
    this.SliderVText_Text = SliderVText:GetComponent("UnityEngine.UI.Text")
    this.SheetContent_RectTransform = SheetContent:GetComponent("UnityEngine.RectTransform")
    this.sheet_tab_LuaBehaviour = sheet_tab:GetComponent("LuaBehaviour")
    this.SliderColum_Slider = SliderColum:GetComponent("UnityEngine.UI.Slider")
    this.SliderColumText_Text = SliderColumText:GetComponent("UnityEngine.UI.Text")
end
--AutoGenInit End

function this.Awake()
    this.AutoGenInit()
    print("this.Awake")
-- end

-- function this.OnEnable()
--     print("this.OnEnable")
-- end

-- function this.Start()
    print("this.Start")

    xlua.private_accessible(CS.TableView.TableView)

    this.tableview_TableViewController.GetDataCount = function(table)
        -- return this.dataSource.LastRowNum - this.dataSource.FirstRowNum
        return #this.dataSource
    end
    this.tableview_TableViewController.GetCellSize = function(table, row)
        -- print("GetCellSize: "..row)
        local size = 40;
        local hightperline = 40
        local excel_row = this.dataSource[row + 1]
        -- for i = this.FirstCellNum, excel_row.LastCellNum do end
        for i = this.ColumnIdxA, this.ColumnIdxA + this.ColumnPerPage do
            local cell = excel_row:GetCell(i)
            if cell ~= nil then
                local s = cell:StrValue()
                local ss = s:split('\n')
                local lc = 1
                for i,v in ipairs(ss) do
                    lc = lc + 1 + math.ceil(#v / 3 * 40 /(1700 / this.ColumnPerPage))
                end
                local tmp = lc * hightperline
                if tmp > size then size = tmp end
            end
        end

        -- if table.tableView.LayoutOrientation == 1 then
        --     size = cell.rectTransform.height
        -- else
        --     size = cell.rectTransform.width
        -- end
        return size
    end
    this.tableview_TableViewController.CellAtRow = function(tb, row)
        this.SliderV_Slider.value = this.tableview_TableViewController.tableView.Position / this.tableview_TableViewController.tableView.ContentSize
        local cell
        if(row % 3) == 0 then
            cell = tb:ReusableCellForRow("table_view_cell_01", row)
        elseif(row % 3) == 1 then
            cell = tb:ReusableCellForRow("table_view_cell_02", row)
        elseif(row % 3) == 2 then
            cell = tb:ReusableCellForRow("table_view_cell_03", row)
        end
        cell.name = "lua-Cell-" ..(row)
        local cellmono = cell:GetComponent("LuaBehaviour")
        -- cellmono.luaTable.SetData(this.dataSource[row+1])
        cellmono.luaTable.SetExcelCellData(this.dataSource[row + 1], this.ColumnIdxA, this.ColumnPerPage)
        -- print("CellAtRow", row)
        return cell
    end

    local subpath = "Streaming/Excel.tmp/StringDB-diff-1.10-2.2-zh.xlsx"
    local assetPath = "/Users/cn/Documents/2-中级预言成就顺理.xlsx" -- CS.AssetSys.CacheRoot .. "../" .. subpath
    -- local assetPath = CS.UnityEngine.Application.streamingAssetsPath .. "/Excel.tmp/EtudeLessonInfo.xlsx"
    local book = CS.ExcelUtils.Open(assetPath);
    print("open", book)
    this.Sheets = {}
    this.SheetTabs = {}
    local selcolor = {r = 0, g = 123, b = 100, a = 255}
    local restcolor = {r = 255, g = 255, b = 255, a = 255}
    for i = 0, book.NumberOfSheets - 1 do
        print(i, book[i].SheetName)
        this.Sheets[i + 1] = {
            name = book[i].SheetName,
            sheet = book[i]
        }
        -- sheet tab
        local sheet = this.Sheets[i + 1].sheet
        local new_sheet_tab = GameObject.Instantiate(sheet_tab)
        new_sheet_tab.name = "sheet_tab_" .. sheet.SheetName
        new_sheet_tab.transform:SetParent(SheetContent.transform)
        local lua_sheet_tab = new_sheet_tab:GetComponent("LuaBehaviour").luaTable
        -- lua_sheet_tab.SetData(excel_view, sheet)
        lua_sheet_tab.Text_Text.text = sheet.SheetName
        lua_sheet_tab.Button_Button.onClick:AddListener(function()
            for k, v in pairs(this.SheetTabs) do
                v.Button_Image.color = restcolor
            end
            lua_sheet_tab.Button_Image.color = selcolor
            this.initSheetData(sheet)
            this.tableview_TableView:ReloadData()
        end)
        table.insert(this.SheetTabs, lua_sheet_tab)
        SheetContent.transform.sizeDelta = {x = book.NumberOfSheets * 155, y = 50}
    end -- for
    GameObject.DestroyImmediate(sheet_tab)
    this.SheetTabs[1].Button_Image.color = selcolor

    if #this.Sheets > 0 then
        local sheet = this.Sheets[1].sheet
        this.initSheetData(sheet)
        -- this.tableview_TableView:ReloadData()
        this.SliderColum_Slider.value = this.ColumnPerPage / this.ColumnPerPageMax
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

    this.grep_Button.onClick:AddListener(function()
        -- this.grepData()
        this.ColumnIdxA = this.ColumnIdxA - 1
        if this.ColumnIdxA < 0 then
            this.ColumnIdxA = 0
        end
        this.Slider_Slider.value = this.ColumnIdxA / 10
        this.tableview_TableViewController.tableView:ReloadData()
    end)
    this.SliderColum_Slider.onValueChanged:AddListener(function(fval)
        this.ColumnPerPage = math.modf(fval * this.ColumnPerPageMax)
        this.SliderColumText_Text.text = this.ColumnPerPageMax
        this.tableview_TableViewController.tableView:ReloadData()
    end)
    this.Slider_Slider.onValueChanged:AddListener(function(fval)
        this.ColumnIdxA = math.modf(this.Slider_Slider.value * 10)
        -- print("onValueChanged", fval, this.Slider_Slider.value, this.ColumnIdxA)
        this.tableview_TableViewController.tableView:ReloadData()
    end)
    this.SliderV_Slider.onValueChanged:AddListener(function(fval)
        -- print("SliderV_Slider.onValueChanged", fval, this.SliderV_Slider.value)
        this.SliderVText_Text.text = string.format("%.0f", fval * 100)
        this.tableview_TableViewController.tableView:SetPosition(fval * this.tableview_TableViewController.tableView.ContentSize)
    end)

    this.back_Button.onClick:AddListener(function()
        this.Back()
    end)
end

function this.initSheetData(sheet)
    this.dataSource = {}
    print("initSheetData", sheet.FirstRowNum, sheet.LastRowNum)
    for i = sheet.FirstRowNum, sheet.LastRowNum do
        table.insert(this.dataSource, sheet[i])
    end
    this.count_Text.text = #this.dataSource
end

function this.Back()
    assert(coroutine.resume(coroutine.create(function()
        yield_return(UnityEngine.WaitForSeconds(0.3))
        local obj = nil
        yield_return(CS.AssetSys.GetAsset("ui/test/test.prefab", function(asset)
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