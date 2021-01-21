-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite3 = require("lsqlite3")
local manager = manager

local print = function ( ... )
    _G.print("writeplayer.index.cell", ... )
end

local util = require "xlua.util"

local cell = {
    id = nil,
    name = nil,
    url = nil,
    cpath = nil,
    tpath = nil,
    tid = nil
}
local this = cell
local self = cell

local yield_return = util.async_to_sync(function (to_yield, callback)
     mono:YieldAndCallback(to_yield, callback)
 end)

-- function this.coroutine_demo()
--     return coroutine.create(function()
--         print('table_view_cell coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.TableViewCell = gameObject:GetComponent(typeof(CS.TableView.TableViewCell))
    this.Animator = gameObject:GetComponent(typeof(CS.UnityEngine.Animator))
    this.DelBtn_Button = DelBtn:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.Image_Image = Image:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.Text_Text = Text:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_1_Text = Text_1:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_2_Text = Text_2:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_3_Text = Text_3:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_4_Text = Text_4:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function this.Awake()
    this.AutoGenInit()
end

function this.Start()
    local ct = cell
    ct.TableViewCell:DidSelectEvent("+", function(row2)
        print("cellevent DidSelectEvent", row2, ct.TableViewthis.RowNumber)
    end)
    ct.TableViewCell:DidPointClickEvent("+", function(row2)
        print("cellevent DidPointClickEvent", row2, this.RowNumber, ct.Animator, ct.Animator.Play)
        ct.Animator:Play("cell_scale", 0)
        --this.Play()
        manager.Scene.push("writeplayer/player/player.prefab",function(player)
            local playermono = player:GetComponent(typeof(CS.LuaMonoBehaviour))
            local ct = playermono.Lua
            ct.SetData(this.data)
        end)
    end)
    ct.TableViewCell:DidHighlightEvent("+", function(row2)
        print("cellevent DidHighlightEvent", row2, this.RowNumber)
    end)
end


function this.SetCellData(data, columnidx, num)
    if data == nil then
        return
    end
    --if num <= 0 then num = 1 end
    this.data = data
    print("this.SetCellData", data.id, data.url)
    
    self.Text_Text.text = data.id
    --[[
    {
        id    = reader:GetInt32(0),
        name   = reader:GetInt32(1),
        url   = reader:GetTextReader(2):ReadToEnd(),
        cpath = reader:GetTextReader(3):ReadToEnd(),
        tpath = reader:GetTextReader(4):ReadToEnd()
    }
    ]]
    self.Text_1_Text.text = data.name
    self.Text_2_Text.text = data.url
    self.Text_3_Text.text = data.cpath
    self.Text_4_Text.text = data.tpath
    --for i = 1, 7 do 
    --    local key = "Text_"..i.. "_Text"
    --    if i > num then
    --        self[key].gameObject:SetActive(false)
    --    else
    --        self[key].gameObject:SetActive(true) 
    --        self[key].transform.sizeDelta = {x = (mono.gameObject.transform.sizeDelta.x-100) / num, y = 80}
    --        local cell = row:GetCell(columnidx + i - 1)
    --        if cell ~= nil then
    --            local s = cell:StrValue()
    --            self[key].text = string.gsub(s, "\\n", "\n") .. ":" .. #s
    --        else
    --            self[key].text = "~nil~"
    --        end
    --    end
    --end
end


return cell
