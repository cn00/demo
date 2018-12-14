-- put this to path/to/unity3d/Editor/{Data|Contents}/Resources/ScriptTemplates/87-LuaScript-NewLuaScript.lua.txt

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "utility.xlua.util"

local sheet_tab = {}
local this = sheet_tab

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)

-- function sheet_tab.coroutine_demo()
--     return coroutine.create(function()
--         print('sheet_tab coroutine start!')
--         yield_return(UnityEngine.WaitForSeconds(1))
--         local obj = nil
--         yield_return(CS.AssetSys.Instance:GetAsset("ui/login/login.prefab", function(asset)
--             obj = asset
--         end))
--         local gameObj = GameObject.Instantiate(obj)
--     end)
-- end

--AutoGenInit Begin
function sheet_tab.AutoGenInit()
    sheet_tab.Button_Button = Button:GetComponent("UnityEngine.UI.Button")
    sheet_tab.Text_Text = Text:GetComponent("UnityEngine.UI.Text")
    sheet_tab.Button_Image = Button:GetComponent("UnityEngine.UI.Image")
end
--AutoGenInit End

function sheet_tab.Awake()
	this.AutoGenInit()
end

-- local selcolor = {r=0, g = 123, b = 100, a = 255}
-- local restcolor = {r=255, g = 255, b = 255, a = 255}
-- function sheet_tab.SetData(table_view, sheet)
--     this.data = sheet
--     this.Text_Text.text = sheet.SheetName
--     this.Button_Button.onClick:AddListener(function()
--         for k, v in pairs(table_view.SheetTabs) do
--             v.Button_Image.color = restcolor
--         end
--         this.Button_Image.color = selcolor
--         table_view.initSheetData(sheet)
--         table_view.tableview_TableViewController.tableView:ReloadData()
--     end)
-- end

function sheet_tab.OnEnable()
    print("sheet_tab.OnEnable")

end

function sheet_tab.Start()
    print("sheet_tab.Start")

    --assert(coroutine.resume(sheet_tab.coroutine_demo()))

end

function sheet_tab.FixedUpdate()

end

function sheet_tab.Update()

end

function sheet_tab.LateUpdate()

end

function sheet_tab.OnDestroy()
    print("sheet_tab.OnDestroy")

end

function sheet_tab.Destroy()
    GameObject.DestroyImmediate(mono.gameObject)
end

return sheet_tab
