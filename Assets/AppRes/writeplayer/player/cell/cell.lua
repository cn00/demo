
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "xlua.util"

-- cell

local print = function ( ... )
    _G.print("cell", ...)
end

local this = {}

-- local yield_return = util.async_to_sync(function (to_yield, callback)
--     mono:YieldAndCallback(to_yield, callback)
-- end)
-- function this.coroutine_demo()
--     print('coroutine start!')
--     yield_return(UnityEngine.WaitForSeconds(1))
--     local obj = nil
--     yield_return(CS.AssetSys.GetAsset("ui/login/login.prefab", function(asset)
--         obj = asset
--     end))
--     local gameObj = GameObject.Instantiate(obj)
-- end

--AutoGenInit Begin
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.TableViewCell = gameObject:GetComponent(typeof(CS.TableView.TableViewCell))
    this.Image_Image = Image:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.Text_Text = Text:GetComponent(typeof(CS.UnityEngine.UI.Text))
end
--AutoGenInit End

function this.Awake()
	this.AutoGenInit()
end

-- function this.OnEnable() end


function this.SetCellData(data, columnidx, num)
	if data == nil then
		return
	end
	--if num <= 0 then num = 1 end
	this.data = data
	--print("this.SetCellData", data.c)

	this.Text_Text.text = data.c
end
return this
