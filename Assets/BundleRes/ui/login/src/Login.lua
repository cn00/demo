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
	    local loading = CS.UnityEngine.GameObject.Instantiate(obj)
	    loading.name = "loading"
	    local oldLoading = CS.UnityEngine.GameObject.Find("loading")
	    CS.UnityEngine.GameObject.DestroyImmediate(oldLoading)
	    

	    obj = nil
	    yield_return(CS.AssetSys.Instance:GetAsset("ui/update/update.prefab", function(asset)
	        obj = asset;
	    end))
	    local update = CS.UnityEngine.GameObject.Instantiate(obj)
	    update.name = "update"

	end)
 end

--AutoGenInit Begin
function Login.AutoGenInit()
    Login.Button = Button:GetComponent("UnityEngine.UI.Button")
    Login.InputField = InputField:GetComponent("UnityEngine.UI.InputField")
    Login.InputField_1 = InputField_1:GetComponent("UnityEngine.UI.InputField")
end
--AutoGenInit End

function update.Awake()
	Login.AutoGenInit()
	
	Login.Button.onClick:AddListener(function()
		print("clicked, you input is [" .. InputField:GetComponent("InputField").text .."]")
		assert(coroutine.resume(Login.CheckUpdate()))
	end)
end

function Login.Start()
	print("lua Login.start..."..mono.transform.position:ToString())

end

function Login.Update ()
	-- print("Login.Update")
end

function Login.OnDestroy ()

end

return Login
