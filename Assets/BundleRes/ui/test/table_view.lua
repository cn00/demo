-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local testData = {
    {
        id = 2,
        name = "name_2",
        text = "text",
        "string",
        23456,
        "string2",
        [80] = 888889999,
        {
            i = 9,
            s = "9999"
        },
        [55] = {
            i = 333,
            s = "2222"
        },
        {
            i = 444,
            s = "222444"
        },
    },
    {
        id = 1,
        name = "name_1",
        text = "text",
    },
    {
        id = 2,
        name = "name_2",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 4,
        name = "name_3",
        text = "text",
    },
    {
        id = 5,
        name = "name_3",
        text = "text",
    },
    {
        id = 6,
        name = "name_3",
        text = "text",
    },
    {
        id = 7,
        name = "name_3",
        text = "text",
    },
    {
        id = 8,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 1,
        name = "name_1",
        text = "text",
    },
    {
        id = 2,
        name = "name_2",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 1,
        name = "name_1",
        text = "text",
    },
    {
        id = 2,
        name = "name_2",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 1,
        name = "name_1",
        text = "text",
    },
    {
        id = 2,
        name = "name_2",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
    {
        id = 3,
        name = "name_3",
        text = "text",
    },
}


local table_view = {
    RowIdxA = 1,
    RowPerPage = 10,
    ColumnIdxA = 0,

    dataSource = {}
}
local self = table_view

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

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
    table_view.table_TableView = table:GetComponent("TableView.TableView")
    table_view.table_TableViewController = table:GetComponent("TableView.TableViewController")
    table_view.grep_Button = grep:GetComponent("UnityEngine.UI.Button")
    table_view.reset_Button = reset:GetComponent("UnityEngine.UI.Button")
    table_view.count_Text = count:GetComponent("UnityEngine.UI.Text")
    table_view.back_Button = back:GetComponent("UnityEngine.UI.Button")
    table_view.Slider_Slider = Slider:GetComponent("UnityEngine.UI.Slider")
    table_view.SliderV_Slider = SliderV:GetComponent("UnityEngine.UI.Slider")
    table_view.SliderVText_Text = SliderVText:GetComponent("UnityEngine.UI.Text")
end
--AutoGenInit End

function table_view.initData()
    table_view.dataSource = {}
    for i = 1, #testData do
        _G.table.insert(table_view.dataSource, testData[i])
    end
    table_view.count_Text.text = #table_view.dataSource
end

function table_view.grepData()
    table_view.dataSource = {}
    for i = 1, #testData do
        if testData[i].id > 3 then
            table.insert(table_view.dataSource, testData[i])
        end
    end
    table_view.count_Text.text = #table_view.dataSource
end

function table_view.initSheetData(sheet)
    table_view.dataSource = {}
    print(sheet.FirstRowNum, sheet.LastRowNum)
    for i = sheet.FirstRowNum, sheet.LastRowNum do
            _G.table.insert(table_view.dataSource, sheet[i])
    end
    table_view.count_Text.text = #table_view.dataSource
end


function table_view.Awake()
    self.AutoGenInit()

    xlua.private_accessible(CS.TableView.TableView)

    table_view.table_TableViewController.GetDataCount = function(table)
    --    return table_view.dataSource.LastRowNum - table_view.dataSource.FirstRowNum
       return #table_view.dataSource
    end
    table_view.table_TableViewController.GetCellSize = function(table, row)
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
    table_view.table_TableViewController.CellAtRow = function(table, row)
        table_view.SliderV_Slider.value = table_view.table_TableViewController.tableView.Position / table_view.table_TableViewController.tableView.ContentSize
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
        cellmono.luaTable.SetExcelCellData(table_view.dataSource[row+1], table_view.ColumnIdxA)
        -- print("CellAtRow", row)
        return cell
    end

    local subpath = "StreamingAssets/Excel.tmp/EtudeLessonInfo.xlsx"
    local assetPath = CS.AssetSys.CacheRoot .. "download/" .. subpath
    -- local assetPath = CS.UnityEngine.Application.streamingAssetsPath .. "/Excel.tmp/EtudeLessonInfo.xlsx"
    local book = CS.ExcelUtils.Open(assetPath);
    table_view.Sheets = {}
    for i = 0, book.NumberOfSheets - 1 do
        print(i, book[i].SheetName)
        table_view.Sheets[i+1] = {
            name = book[i].SheetName, 
            sheet = book[i]
        }
    end
    if #table_view.Sheets > 0 then
        local sheet = table_view.Sheets[1].sheet
        table_view.initSheetData(sheet)
    end

    -- table_view.initData()
    -- table_view.table_TableViewController.tableView:ReloadData()

    table_view.reset_Button.onClick:AddListener(function()
        -- table_view.initData()
        table_view.ColumnIdxA = table_view.ColumnIdxA + 1
        if table_view.ColumnIdxA > 10 then
            table_view.ColumnIdxA = 10
        end
        table_view.Slider_Slider.value = table_view.ColumnIdxA / 10.0

        table_view.table_TableViewController.tableView:ReloadData()
    end)

    table_view.grep_Button.onClick:AddListener(function()
        -- table_view.grepData()

        table_view.ColumnIdxA = table_view.ColumnIdxA - 1
        if table_view.ColumnIdxA < 0 then
            table_view.ColumnIdxA = 0
        end
        table_view.Slider_Slider.value = table_view.ColumnIdxA / 10.0
        table_view.table_TableViewController.tableView:ReloadData()
    end)
    table_view.Slider_Slider.onValueChanged:AddListener(function(fval)
        table_view.ColumnIdxA = math.modf(table_view.Slider_Slider.value * 10)
        print("onValueChanged", fval, table_view.Slider_Slider.value, table_view.ColumnIdxA)
        table_view.table_TableViewController.tableView:ReloadData()
    end)
    table_view.SliderV_Slider.onValueChanged:AddListener(function(fval)
        print("SliderV_Slider.onValueChanged", fval, table_view.SliderV_Slider.value)
        table_view.SliderVText_Text.text = string.format("%.0f", fval * 100)
        table_view.table_TableViewController.tableView:SetPosition(fval*table_view.table_TableViewController.tableView.ContentSize)
    end)

    table_view.back_Button.onClick:AddListener(function()
        -- table_view.initData()
        -- table_view.ColumnIdxA = table_view.ColumnIdxA + 1
        -- if table_view.ColumnIdxA > 10 then
        --     table_view.ColumnIdxA = 10
        -- end
    end)
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
