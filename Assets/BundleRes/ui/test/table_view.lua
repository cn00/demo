-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local table_view = {
    data = {
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
    table_view.Button_Button = Button:GetComponent("UnityEngine.UI.Button")
end
--AutoGenInit End

function table_view.Awake()
    self.AutoGenInit()
    table_view.table_TableViewController.GetDataCount = function(table)
       return #table_view.data
    end
    table_view.table_TableViewController.GetCellSize = function(table, row)
        -- local cell = table:ReusableCellForRow("table-view-cell-01", row)
        local size = 250;
        -- if table.tableView.LayoutOrientation == 1 then
        --     size = cell.rectTransform.height
        -- else
        --     size = cell.rectTransform.width
        -- end
        print("GetCellSize: "..row)
        return size
    end
    table_view.table_TableViewController.CellAtRow = function(table, row)
        local cell
        if (row % 2) == 0 then
            cell = table:ReusableCellForRow("table-view-cell-01", row)
        else
            cell = table:ReusableCellForRow("table-view-cell-02", row)
       end
        cell.name = "lua-Cell-" .. (row+1)
        local cellmono = cell:GetComponent("LuaMonoBehaviour")
        cellmono.luaTable.SetData(table_view.data[row+1])
        -- print("CellAtRow", row)
        return cell
    end

    table_view.Button_Button.onClick:AddListener(function()
        table_view.grep = true
        table_view.databak = table_view.data
        table_view.data = {}
        for i = 1, #table_view.databak do
            if table_view.databak[i].id > 3 then
                _G.table.insert(table_view.data, table_view.databak[i])
            end
        end
        table_view.table_TableViewController.tableView:ReloadData()
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
