local util = require "lua.utility.xlua.util"
--local LoginHelper = require 'LoginHelper'

local Login = {
	date1=123456
}

local yield_return = util.async_to_sync(function (to_yield, cb)
	mono:YieldAndCallback(to_yield, cb)
end)

function Login.CheckUpdate()
	return coroutine.create(function()
		yield_return(CS.UpdateSys.Instance:CheckUpdate())

	    local obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset("ui/loading/loading.prefab", function(asset)
	        obj = asset;
	    end))
	    local oldLoading = CS.UnityEngine.GameObject.Find("loading")
	    CS.UnityEngine.GameObject.DestroyImmediate(oldLoading)
	    
	    local loading = CS.UnityEngine.GameObject.Instantiate(obj)
	    loading.name = "loading"
	end)
 end
-- local self = Login
function Login:init( ... )
	print("Login.init" .. self.date1)
	-- body
end

function Login.Start()
	print("lua Login.start..."..mono.transform.position:ToString())
	Login:init()
	
	Button:GetComponent("Button").onClick:AddListener(function()
		print("clicked, you input is '" .. InputField:GetComponent("InputField").text .."'")
		assert(coroutine.resume(Login.CheckUpdate()))
	end)

end

function Login.Update ()
	-- print("Login.Update")
end

function Login.OnDestroy ()

end

return Login
