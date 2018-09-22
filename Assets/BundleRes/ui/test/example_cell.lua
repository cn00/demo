-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"

local example_cell = {
    ColumnPerPage = 5,
}
local self = example_cell

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function example_cell.coroutine_demo()
--     return coroutine.create(function()
--         print('example_cell coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function example_cell.AutoGenInit()
    example_cell.table_view_cell_01_TableViewCellController = mono.gameObject:GetComponent("TableView.TableViewCellController")
    example_cell.Image_Image = Image:GetComponent("UnityEngine.UI.Image")
    example_cell.Text_Text = Text:GetComponent("UnityEngine.UI.Text")
    example_cell.Text_1_Text = Text_1:GetComponent("UnityEngine.UI.Text")
    example_cell.Text_2_Text = Text_2:GetComponent("UnityEngine.UI.Text")
    example_cell.Text_3_Text = Text_3:GetComponent("UnityEngine.UI.Text")
    example_cell.Text_4_Text = Text_4:GetComponent("UnityEngine.UI.Text")
    example_cell.Text_5_Text = Text_5:GetComponent("UnityEngine.UI.Text")
    example_cell.Text_6_Text = Text_6:GetComponent("UnityEngine.UI.Text")
    example_cell.Text_7_Text = Text_7:GetComponent("UnityEngine.UI.Text")
end
--AutoGenInit End

function example_cell.Awake()
    self.AutoGenInit()
    
    example_cell.TableViewCellController = example_cell.table_view_cell_01_TableViewCellController

    example_cell.TableViewCellController:DidSelectEvent("+", function(row)
        print(row .. " DidSelectEvent")
    end)
    example_cell.TableViewCellController:DidPointClickEvent("+", function(row)
        print(row .. " DidPointClickEvent" .. ":" .. example_cell.TableViewCellController.RowNumber)
    end)
    example_cell.TableViewCellController:DidHighlightEvent("+", function(row)
        print(row .. " DidHighlightEvent")
    end)
end

-- function example_cell.OnEnable()
--     print("example_cell.OnEnable")

-- end

function example_cell.SetData(data)
    self.data = data
    print("example_cell.SetData", data.name)
    self.Text_Text.text = self.data.name .. ":" .. example_cell.TableViewCellController.RowNumber
end

function example_cell.SetExcelCellData(row, columnidx, num)
    if row == nil then
        return
    end
    self.data = row
    -- print("example_cell.SetExcelCellData", row:GetCell(columnidx))
    self.Text_Text.text = example_cell.TableViewCellController.RowNumber
    for i = 1, 7 do 
        local key = "Text_"..i.. "_Text"
        -- print(key)
        local cell = row:GetCell(columnidx + i - 1)
        if cell ~= nil then
            self[key].text = cell:StrValue()
        else
            self[key].text = "~nil~"
        end
    end
end

-- function example_cell.Start()
--     print("example_cell.Start", self.data.name)

--     --assert(coroutine.resume(example_cell.coroutine_demo()))

-- end

-- function example_cell.FixedUpdate()

-- end

-- function example_cell.Update()

-- end

-- function example_cell.LateUpdate()

-- end

-- function example_cell.OnDestroy()
--     print("example_cell.OnDestroy")

-- end

function example_cell.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return example_cell
