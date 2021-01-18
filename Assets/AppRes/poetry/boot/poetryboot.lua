
--- Author: cn
--- Email: cool_navy@qq.com
--- Date: 2021/01/08 18:45:31
--- Description: 
--[[

]]

local G = _G
local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local util = require "lua.utility.xlua.util"
require("lua.utility.BridgingClass")

-- poetryboot

local print = function ( ... )
    _G.print("[poetryboot]", ...)
end

local poetryboot = {}
local this = poetryboot


local yield_return = util.async_to_sync(function(to_yield, callback)
	mono:YieldAndCallback(to_yield, callback)
end)

--AutoGenInit Begin
function poetryboot.AutoGenInit() end
--AutoGenInit End

function poetryboot.Awake()
	this.AutoGenInit()
end

-- function poetryboot.OnEnable() end

function poetryboot.Start()
	util.coroutine_call(function(...)
		print("boot.coroutine_boot")

		local obj = nil
		yield_return(CS.AssetSys.GetAsset("common/manager/manager.prefab", function(asset)
			obj = asset
		end))
		GameObject.Instantiate(obj)

		yield_return(CS.AssetSys.GetAsset("font/fzxz/方正小篆体.ttf"))
		
	end)
end



function poetryboot.Destroy()
	GameObject.DestroyImmediate(mono.gameObject)
end

return poetryboot
