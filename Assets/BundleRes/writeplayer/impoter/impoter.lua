local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys
local sqlite = CS.SQLite.SQLite3

local util = require "lua.utility.xlua.util"

local import = {
}

local self = import
local this = import

local Game = _G.Game


local print = function(...)
	_G.print("[importer]", ...)
end

local yield_return =
	util.async_to_sync(
	function(to_yield, callback)
		mono:YieldAndCallback(to_yield, callback)
	end
)

function import.Submit(...)
	print(debug.traceback("test traceback Submit "))
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
--DO NOT EDIT THIS FUNCTION MANUALLY.
function this.AutoGenInit()
    this.submit_Button = submit:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.title_InputField = title:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.url_InputField = url:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.content_InputField = content:GetComponent(typeof(CS.UnityEngine.UI.InputField))
end
--AutoGenInit End

function import.Awake()
	self.AutoGenInit()


	--local dbpath = ""
	--this.db = sqlite3.open(dbpath)

	self.title_InputField.onEndEdit:AddListener(function(text)
		print("title_InputField.onEndEdit:" .. text)
		this.titlestr = text
		
		self.url_InputField:Select()
	end)
	self.url_InputField.onEndEdit:AddListener(function(text)
		print("url_InputField.onEndEdit:" .. text)
		local exist = AssetSys.UrlIsExist(text)
		this.urlstr = text
		
		self.content_InputField:Select()
	end)
	self.content_InputField.onEndEdit:AddListener(function(text)
		print("content_InputField.onEndEdit:" .. text)
		this.contentstr = text
	end)

	self.submit_Button.onClick:AddListener(function(...)
		-- todo: check storaged name url content
	
		--assert(coroutine.resume(self.Submit()))
		local sql = string.format([[
			insert into item_view (title, url, content) 
			value("%s","%s","%s")
		]],this.titlestr, this.urlstr, this.contentstr)
		print("submit sql:", sql)

--		this.db:exec(sql)
	end)

	-- -- test message center
	-- local global = GameObject.Find("Global")
	-- gmono = global:GetComponent("LuaMonoBehaviour")
	-- gmono.luaTable.import = self
end

-- function import.OnEnable()end

-- function import.Start()end

-- function import.FixedUpdate()end

-- function import.Update()end

-- function import.LateUpdate()end

-- function import.OnDestroy()end

return import
