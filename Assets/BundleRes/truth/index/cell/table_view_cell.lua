-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

local table_view_cell = {}
local self = table_view_cell

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function table_view_cell.coroutine_demo()
--     return coroutine.create(function()
--         print('table_view_cell coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function table_view_cell.AutoGenInit()
    table_view_cell.TableViewCellController = mono.gameObject:GetComponent("TableView.TableViewCellController")
    table_view_cell.Image_Image = Image:GetComponent("UnityEngine.UI.Image")
    table_view_cell.Text_Text = Text:GetComponent("UnityEngine.UI.Text")
    table_view_cell.Text_1_Text = Text_1:GetComponent("UnityEngine.UI.Text")
    table_view_cell.Text_2_Text = Text_2:GetComponent("UnityEngine.UI.Text")
    table_view_cell.Text_3_Text = Text_3:GetComponent("UnityEngine.UI.Text")
    table_view_cell.Text_4_Text = Text_4:GetComponent("UnityEngine.UI.Text")
    table_view_cell.Text_5_Text = Text_5:GetComponent("UnityEngine.UI.Text")
    table_view_cell.Text_6_Text = Text_6:GetComponent("UnityEngine.UI.Text")
    table_view_cell.Text_7_Text = Text_7:GetComponent("UnityEngine.UI.Text")
end
--AutoGenInit End

function table_view_cell.Awake()
    self.AutoGenInit()
end

function table_view_cell.Start()

    table_view_cell.TableViewCellController:DidSelectEvent("+", function(row)
        print(row .. " DidSelectEvent")
    end)
    table_view_cell.TableViewCellController:DidPointClickEvent("+", function(row)
        print(row .. " DidPointClickEvent" .. ":" .. table_view_cell.TableViewCellController.RowNumber)
    end)
    table_view_cell.TableViewCellController:DidHighlightEvent("+", function(row)
        print(row .. " DidHighlightEvent")
    end)
end


function table_view_cell.SetExcelCellData(row, columnidx, num)
    if row == nil then
        return
    end
    if num <= 0 then num = 1 end
    self.data = row
    -- print("table_view_cell.SetExcelCellData", row:GetCell(columnidx))
    self.Text_Text.text = table_view_cell.TableViewCellController.RowNumber
    for i = 1, 7 do 
        local key = "Text_"..i.. "_Text"
        if i > num then
            self[key].gameObject:SetActive(false)
        else
            self[key].gameObject:SetActive(true) 
            self[key].transform.sizeDelta = {x = (mono.gameObject.transform.sizeDelta.x-100) / num, y = 80}
            local cell = row:GetCell(columnidx + i - 1)
            if cell ~= nil then
                local s = cell:StrValue()
                self[key].text = string.gsub(s, "\\n", "\n") .. ":" .. #s
            else
                self[key].text = "~nil~"
            end
        end
    end
end

-- end

-- function table_view_cell.FixedUpdate()

-- end

-- function table_view_cell.Update()

-- end

-- function table_view_cell.LateUpdate()

-- end

-- function table_view_cell.OnDestroy()
--     print("table_view_cell.OnDestroy")

-- end

function table_view_cell.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return table_view_cell
