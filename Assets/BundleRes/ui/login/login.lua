

local util = require "lua.utility.xlua.util"

local login = {}
local self = login

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)


function login.CheckUpdate()
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
function login.AutoGenInit()
    login.Button = Button:GetComponent("UnityEngine.UI.Button")
    login.InputField = InputField:GetComponent("UnityEngine.UI.InputField")
    login.InputField_1 = InputField_1:GetComponent("UnityEngine.UI.InputField")
end
--AutoGenInit End

function login.Awake()
	login.AutoGenInit()
	login.Button.onClick:AddListener(function()
		print("clicked, you input is [" .. InputField:GetComponent("InputField").text .."]")
		assert(coroutine.resume(login.CheckUpdate()))
	end)
end

function login.OnEnable()
    print("login.OnEnable")

end

function login.Start()
    print("login.Start")
end

function login.FixedUpdate()

end

function login.Update()

end

function login.LateUpdate()

end

function login.OnDestroy()
    print("login.OnDestroy")

end
    
return login
