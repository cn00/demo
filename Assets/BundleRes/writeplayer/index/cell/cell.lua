-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite3 = require("lsqlite3")
local manager = manager

local print = function ( ... )
    _G.print("[writeplayer.index.cell]", ... )
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
local self = cell
local this = self
 local yield_return = util.async_to_sync(function (to_yield, callback)
     mono:YieldAndCallback(to_yield, callback)
 end)

-- function cell.coroutine_demo()
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
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.TableViewCell = gameObject:GetComponent(typeof(CS.TableView.TableViewCell))
    this.Image_Image = Image:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.Text_Text = Text:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_1_Text = Text_1:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_2_Text = Text_2:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_3_Text = Text_3:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_4_Text = Text_4:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_5_Text = Text_5:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_6_Text = Text_6:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Text_7_Text = Text_7:GetComponent(typeof(CS.UnityEngine.UI.Text))
    this.Animator = gameObject:GetComponent(typeof(CS.UnityEngine.Animator))
end
--AutoGenInit End

function cell.Awake()
    self.AutoGenInit()
end

function cell.Start()
    local ct = cell
    ct.TableViewCell:DidSelectEvent("+", function(row2)
        print("cellevent DidSelectEvent", row2, ct.TableViewCell.RowNumber)
    end)
    ct.TableViewCell:DidPointClickEvent("+", function(row2)
        print("cellevent DidPointClickEvent", row2, ct.TableViewCell.RowNumber, ct.Animator, ct.Animator.Play)
        ct.Animator:Play("cell_scale", 0)
        --cell.Play()
        manager.Scene.push("writeplayer/player/player.prefab",function(player)
            local playermono = player:GetComponent(typeof(CS.LuaMonoBehaviour))
            local ct = playermono.Lua
            ct.SetData(this.data)
        end)
    end)
    ct.TableViewCell:DidHighlightEvent("+", function(row2)
        print("cellevent DidHighlightEvent", row2, ct.TableViewCell.RowNumber)
    end)
end

function cell.Play()
    --if(this.data.text == nil)then
    --    local dbpath = AssetSys.CacheRoot.."/db.db"
    --    local db = sqlite3.open(dbpath);
    --    local tid = this.data.tid
    --    local sql = "select text from text where id = " .. tid .. ";";
    --    print("select text:", sql)
    --    db:exec(sql, function (ud, ncols, values, names)
    --        print("ncols", unpack(ncols))
    --        print("names", unpack(names))
    --        print("values", unpack(values))
    --        this.data.text = ""
    --        return sqlite3.OK
    --    end)
    --end

    util.coroutine_call(function()

        local obj = nil
        yield_return(AssetSys.Instance:GetAsset("ui/loading/loading.prefab", function(asset)
            obj = asset
        end))
        local loading = GameObject.Instantiate(obj)

        local obj
        yield_return(CS.AssetSys.Instance:GetAsset("writeplayer/player/player.prefab", function(asset)
            obj = asset
        end))
        local player = GameObject.Instantiate(obj)
        local playermono = player:GetComponent("LuaMonoBehaviour")
        local ct = playermono.Lua
        ct.SetData(this.data)

        GameObject.DestroyImmediate(loading)
    end)
end

function cell.SetCellData(data, columnidx, num)
    if data == nil then
        return
    end
    --if num <= 0 then num = 1 end
    self.data = data
    print("cell.SetCellData", data.id, data.url)
    
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

-- end

-- function cell.FixedUpdate()

-- end

-- function cell.Update()

-- end

-- function cell.LateUpdate()

-- end

-- function cell.OnDestroy()
--     print("cell.OnDestroy")

-- end

function cell.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return cell
