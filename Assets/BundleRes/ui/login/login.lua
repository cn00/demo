

local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite = CS.SQLite.SQLite3

local util = require "lua.utility.xlua.util"

local login = {}
local self = login

local yield_return = util.async_to_sync(function (to_yield, callback)
    mono:YieldAndCallback(to_yield, callback)
end)

function login.OpenTest( ... )
	return coroutine.create(function()
	    local obj = nil
		yield_return(CS.AssetSys.Instance:GetAsset("ui/test/test.prefab", function(asset)
	        obj = asset;
	    end))
	    local test = GameObject.Instantiate(obj)
	    test.name = "test"
	    local oldLoading = GameObject.Find("loading")
	    GameObject.DestroyImmediate(oldLoading)

	    GameObject.DestroyImmediate(mono.gameObject)
	end)
end

--AutoGenInit Begin
function login.AutoGenInit()
    login.Login_Button = Login:GetComponent("UnityEngine.UI.Button")
    login.x_InputField = x:GetComponent("UnityEngine.UI.InputField")
    login.y_InputField = y:GetComponent("UnityEngine.UI.InputField")
end
--AutoGenInit End

function login.Awake()
	self.AutoGenInit()

	self.x_InputField.onEndEdit:AddListener(function ( text )
		self.y_InputField:Select()
		print("x_InputField.onEndEdit:"..text)
	end)
	self.y_InputField.onEndEdit:AddListener(function ( text )
		print("y_InputField.onEndEdit:"..text)
	end)

	self.Login_Button.onClick:AddListener(function ( ... )
		assert(coroutine.resume(self.OpenTest()))
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
