-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local table_view = {
    RowIdxA = 1,

    ColumnIdxA = 0,
    ColumnPerPage = 5,
    ColumnPerPageMax = 7,

    dataSource = {}
}
local this = table_view

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

-- function table_view.coroutine_demo()
--     return coroutine.create(function()
--         print('table_view coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function table_view.AutoGenInit()
    table_view.tableview_TableView = tableview:GetComponent("TableView.TableView")
    table_view.tableview_TableViewController = tableview:GetComponent("TableView.TableViewController")
    table_view.grep_Button = grep:GetComponent("UnityEngine.UI.Button")
    table_view.reset_Button = reset:GetComponent("UnityEngine.UI.Button")
    table_view.count_Text = count:GetComponent("UnityEngine.UI.Text")
    table_view.back_Button = back:GetComponent("UnityEngine.UI.Button")
    table_view.Slider_Slider = Slider:GetComponent("UnityEngine.UI.Slider")
    table_view.SliderV_Slider = SliderV:GetComponent("UnityEngine.UI.Slider")
    table_view.SliderVText_Text = SliderVText:GetComponent("UnityEngine.UI.Text")
    table_view.SheetContent_RectTransform = SheetContent:GetComponent("UnityEngine.RectTransform")
    table_view.sheet_tab_LuaMonoBehaviour = sheet_tab:GetComponent("LuaMonoBehaviour")
    table_view.SliderColum_Slider = SliderColum:GetComponent("UnityEngine.UI.Slider")
    table_view.SliderColumText_Text = SliderColumText:GetComponent("UnityEngine.UI.Text")
end
--AutoGenInit End

function table_view.initSheetData(sheet)
    table_view.dataSource = {}
    -- print("initSheetData", sheet.FirstRowNum, sheet.LastRowNum)
    for i = sheet.FirstRowNum, sheet.LastRowNum do
        table.insert(table_view.dataSource, sheet[i])
    end
    table_view.count_Text.text = #table_view.dataSource
end


function table_view.Awake()
    this.AutoGenInit()

    xlua.private_accessible(CS.TableView.TableView)

    table_view.tableview_TableViewController.GetDataCount = function(table)
        -- return table_view.dataSource.LastRowNum - table_view.dataSource.FirstRowNum
        return #table_view.dataSource
    end
    table_view.tableview_TableViewController.GetCellSize = function(table, row)
        -- local cell = table:ReusableCellForRow("table-view-cell-01", row)
        local size = 80;
        -- if table.tableView.LayoutOrientation == 1 then
        --     size = cell.rectTransform.height
        -- else
        --     size = cell.rectTransform.width
        -- end
        -- print("GetCellSize: "..row)
        return size
    end
    table_view.tableview_TableViewController.CellAtRow = function(table, row)
        table_view.SliderV_Slider.value = table_view.tableview_TableViewController.tableView.Position / table_view.tableview_TableViewController.tableView.ContentSize
        local cell
        if (row % 3) == 0 then
            cell = table:ReusableCellForRow("table_view_cell_01", row)
        elseif (row % 3) == 1 then
            cell = table:ReusableCellForRow("table_view_cell_02", row)
        elseif (row % 3) == 2 then
            cell = table:ReusableCellForRow("table_view_cell_03", row)
        end
        cell.name = "lua-Cell-" .. (row+1)
        local cellmono = cell:GetComponent("LuaMonoBehaviour")
        -- cellmono.luaTable.SetData(table_view.dataSource[row+1])
        cellmono.luaTable.SetExcelCellData(table_view.dataSource[row+1], table_view.ColumnIdxA, table_view.ColumnPerPage)
        -- print("CellAtRow", row)
        return cell
    end

    local subpath = "StreamingAssets/Excel.tmp/EtudeLessonInfo.xlsx"
    local assetPath = CS.AssetSys.CacheRoot .. "download/" .. subpath
    -- local assetPath = CS.UnityEngine.Application.streamingAssetsPath .. "/Excel.tmp/EtudeLessonInfo.xlsx"
    local book = CS.ExcelUtils.Open(assetPath);
    print("open", book)
    this.Sheets = {}
    this.SheetTabs = {}
    local selcolor = {r=0, g = 123, b = 100, a = 255}
    local restcolor = {r=255, g = 255, b = 255, a = 255}
    for i = 0, book.NumberOfSheets - 1 do
        print(i, book[i].SheetName)
        this.Sheets[i+1] = {
            name = book[i].SheetName, 
            sheet = book[i]
        }
        -- sheet tab
        local new_sheet_tab = GameObject.Instantiate(sheet_tab)
        new_sheet_tab.name = "sheet_tab_" .. i
        new_sheet_tab.transform:SetParent(SheetContent.transform)
        local lua_sheet_tab = new_sheet_tab:GetComponent("LuaMonoBehaviour").luaTable
        local sheet = table_view.Sheets[i+1].sheet
        lua_sheet_tab.Text_Text.text = sheet.SheetName
        lua_sheet_tab.Button_Button.onClick:AddListener(function()
            for k, v in pairs(table_view.SheetTabs) do
                v.Button_Image.color = restcolor
            end
            lua_sheet_tab.Button_Image.color = selcolor
            this.initSheetData(sheet)
            this.tableview_TableViewController.tableView:ReloadData()
        end)
        table.insert(this.SheetTabs, lua_sheet_tab)
        SheetContent.transform.sizeDelta = {x = book.NumberOfSheets * 155, y = 50}
    end -- for
    GameObject.DestroyImmediate(sheet_tab)
    this.SheetTabs[1].Button_Image.color = selcolor

    if #this.Sheets > 0 then
        local sheet = this.Sheets[1].sheet
        this.initSheetData(sheet)
        this.SliderColum_Slider.value = this.ColumnPerPage / this.ColumnPerPageMax
    end

    -- table_view.initData()
    -- table_view.tableview_TableViewController.tableView:ReloadData()

    table_view.reset_Button.onClick:AddListener(function()
        -- table_view.initData()
        table_view.ColumnIdxA = table_view.ColumnIdxA + 1
        if table_view.ColumnIdxA > 10 then
            table_view.ColumnIdxA = 10
        end
        table_view.Slider_Slider.value = table_view.ColumnIdxA / 10.0

        table_view.tableview_TableViewController.tableView:ReloadData()
    end)

    table_view.grep_Button.onClick:AddListener(function()
        -- table_view.grepData()

        table_view.ColumnIdxA = table_view.ColumnIdxA - 1
        if table_view.ColumnIdxA < 0 then
            table_view.ColumnIdxA = 0
        end
        table_view.Slider_Slider.value = table_view.ColumnIdxA / 10.0
        table_view.tableview_TableViewController.tableView:ReloadData()
    end)
    this.SliderColum_Slider.onValueChanged:AddListener(function(fval)
        this.ColumnPerPage = math.modf(fval * this.ColumnPerPageMax)
        this.SliderColumText_Text.text = this.ColumnPerPageMax
        this.tableview_TableViewController.tableView:ReloadData()
    end) 
    this.Slider_Slider.onValueChanged:AddListener(function(fval)
        this.ColumnIdxA = math.modf(this.Slider_Slider.value * 10)
        -- print("onValueChanged", fval, table_view.Slider_Slider.value, table_view.ColumnIdxA)
        this.tableview_TableViewController.tableView:ReloadData()
    end)
    this.SliderV_Slider.onValueChanged:AddListener(function(fval)
        -- print("SliderV_Slider.onValueChanged", fval, table_view.SliderV_Slider.value)
        this.SliderVText_Text.text = string.format("%.0f", fval * 100)
        this.tableview_TableViewController.tableView:SetPosition(fval*this.tableview_TableViewController.tableView.ContentSize)
    end)

    this.back_Button.onClick:AddListener(function()
       this.Back()
    end)
end

function table_view.Back()
    assert(coroutine.resume(coroutine.create(function()
        yield_return(mono:WaitForSeconds(0.3))
        local obj = nil
        yield_return(CS.AssetSys.Instance:GetAsset("ui/test/test.prefab", function(asset)
            obj = asset
        end))
        local gameObj = GameObject.Instantiate(obj)

	    GameObject.DestroyImmediate(mono.gameObject)
    end)))
end

function table_view.OnEnable()
    print("table_view.OnEnable")

end

function table_view.Start()
    print("table_view.Start")

    --assert(coroutine.resume(table_view.coroutine_demo()))

end

function table_view.FixedUpdate()

end

function table_view.Update()

end

function table_view.LateUpdate()

end

function table_view.OnDestroy()
    print("table_view.OnDestroy")

end

function table_view.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return table_view
