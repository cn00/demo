local CS = CS
local UnityEngine = CS.UnityEngine
local GameObject = UnityEngine.GameObject
local AssetSys = CS.AssetSys

local util = require "lua.utility.xlua.util"
local sqlite3 = require("lsqlite3")

local import = {}

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
    this.BackBtn_Image = BackBtn:GetComponent(typeof(CS.UnityEngine.UI.Image))
    this.content_InputField = content:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.localpath_InputField = localpath:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.submit_Button = submit:GetComponent(typeof(CS.UnityEngine.UI.Button))
    this.title_InputField = title:GetComponent(typeof(CS.UnityEngine.UI.InputField))
    this.url_InputField = url:GetComponent(typeof(CS.UnityEngine.UI.InputField))
end
--AutoGenInit End

function import.Awake()
	self.AutoGenInit()

	local dbpath = AssetSys.CacheRoot .. "db.db"
	local db = sqlite3.open(dbpath)
	this.db = db
	
	self.title_InputField.onEndEdit:AddListener(function(text)
		print("title_InputField.onEndEdit:" .. text)
		this.titlestr = text
		
		self.url_InputField:Select()
	end)
	self.localpath_InputField.onEndEdit:AddListener(function(text)
		print("localpath_InputField.onEndEdit:" .. text)
		--local exist = AssetSys.UrlIsExist(text)
		this.localpath = text
		
		self.content_InputField:Select()
	end)
	self.url_InputField.onEndEdit:AddListener(function(text)
		print("url_InputField.onEndEdit:" .. text)
		--local exist = AssetSys.UrlIsExist(text)
		this.urlstr = text

		self.content_InputField:Select()
	end)
	self.content_InputField.onEndEdit:AddListener(function(text)
		print("content_InputField.onEndEdit:" .. text)
		this.contentstr = string.gsub(text, '"', '\\"')
	end)

	self.submit_Button.onClick:AddListener(function(...)
		-- todo: check storaged name url content

		--util.coroutine_call(function()
			local insert_stmt = assert( this.db:prepare("INSERT INTO item (url, name, text, cpath) values (?,?,?,?)") )
			--yield_return(AssetSys.Instance:GetAsset("writeplayer/impoter/insert.sql", function (asset)
			--	sqlformat = asset.text
			--end))
			insert_stmt:bind_values(this.urlstr, this.titlestr, this.contentstr, this.localpath)
			local _, error = insert_stmt:step()
			print("sql re:", _,  error)
			insert_stmt:reset()
		--end)
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

 function import.OnDestroy()
	 this.db:close()
 end

return import
