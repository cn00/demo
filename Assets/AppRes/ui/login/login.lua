local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite = CS.SQLite.SQLite3

local util = require "xlua.util"

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
				CS.AssetSys.GetAsset(
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
--[[
请勿手动编辑此函数
手動でこの関数を編集しないでください。
DO NOT EDIT THIS FUNCTION MANUALLY.
لا يدويا تحرير هذه الوظيفة
]]
function this.AutoGenInit()
    this.id_InputField = id:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.Login_Button = Login:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.Login_Button.onClick:AddListener(this.Login_OnClick)
    this.pwd_InputField = pwd:GetComponent(typeof(CS.UnityEngine.UI.InputField))
end
--AutoGenInit End

function this.Login_OnClick()
    print('Login_OnClick')
end -- Login_OnClick

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
	-- gmono = global:GetComponent("LuaBehaviour")
	-- gmono.luaTable.login = self
end

-- function login.OnEnable()end

-- function login.Start()end

-- function login.FixedUpdate()end

-- function login.Update()end

-- function login.LateUpdate()end

-- function login.OnDestroy()end

return login