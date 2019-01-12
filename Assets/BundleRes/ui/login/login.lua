local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite = CS.SQLite.SQLite3

local util = require "utility.xlua.util"

local login = {
}

local self = login
local this = login

local Game = _G.Game

local yield_return =
	util.async_to_sync(
	function(to_yield, callback)
		mono:YieldAndCallback(to_yield, callback)
	end
)

function login.OpenTest(...)
	print(debug.traceback("test traceback OpenTest "))
	return coroutine.create(
		function()
			local obj = nil
			yield_return(
				CS.AssetSys.Instance:GetAsset(
					"ui/test/test.prefab",
					function(asset)
						obj = asset
					end
				)
			)
			local test = GameObject.Instantiate(obj)
			test.name = "test"
			local oldLoading = GameObject.Find("loading")
			GameObject.DestroyImmediate(oldLoading)

			GameObject.DestroyImmediate(mono.gameObject)
		end
	)
end

--AutoGenInit Begin
function login.AutoGenInit()
	login.Login_Button = Login:GetComponent("UnityEngine.UI.Button")
	login.id_InputField = id:GetComponent("UnityEngine.UI.InputField")
	login.pwd_InputField = pwd:GetComponent("UnityEngine.UI.InputField")
end
--AutoGenInit End

function login.Awake()
	self.AutoGenInit()

	self.id_InputField.onEndEdit:AddListener(
		function(text)
			self.pwd_InputField:Select()
			print("x_InputField.onEndEdit:" .. text)
		end
	)
	self.pwd_InputField.onEndEdit:AddListener(
		function(text)
			print("y_InputField.onEndEdit:" .. text)
		end
	)

	self.Login_Button.onClick:AddListener(
		function(...)
			assert(coroutine.resume(self.OpenTest()))
		end
	)

	-- -- test message center
	-- local global = GameObject.Find("Global")
	-- gmono = global:GetComponent("LuaMonoBehaviour")
	-- gmono.luaTable.login = self
end

-- function login.OnEnable()end

-- function login.Start()end

-- function login.FixedUpdate()end

-- function login.Update()end

-- function login.LateUpdate()end

-- function login.OnDestroy()end

return login
